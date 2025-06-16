using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using chatlib;
using EI.SI;
using chatapp.data;
using static ProtoIP.Common.Network;
using System.Data.Entity;
using System.Xml;

namespace chatapp
{
    public partial class Chat : Form
    {
        private TcpClient client;
        private NetworkStream stream;
        private ProtocolSI protocol;
        private SerializationHelper helper;
        private User user;
        private DbSetup dbSetup = new DbSetup(DbSetup.CreateConnection());
        private List<Friend> friendsList = new List<Friend>();

        public Chat(User user)
        {
            InitializeComponent();
            this.user = user;

            protocol = new ProtocolSI();
            helper = new SerializationHelper();
            ConnectToServer();
            LoadChat(); // Carrega a lista de amigos do banco de dados local
        }

        private void LoadChat()
        {
            friendsList = dbSetup.GetFriends(DbSetup.CreateConnection(), user.Username);

            // Adciona os amigos na lista de amigos (lb_FriendList)
            foreach (var friend in friendsList)
            {
                lb_FriendList.Items.Add(friend.Username);
            }
        }

        private void LoadUserMsg(string username)
        {
            // Carrega as mensagens do usuário especificado
            List<MessageDataClient> messages = dbSetup.GetMessages(DbSetup.CreateConnection(), username);
            rtb_MessageBoxList.Clear(); // Limpa a caixa de mensagens antes de carregar novas mensagens
            foreach (var message in messages)
            {
                string messageText = $"{message.Timestamp.ToString("HH:mm:ss")} - {message.From}: {message.MessageText}";
                if (!string.IsNullOrEmpty(message.To))
                {
                    messageText += $" (para: {message.To})";
                }
                rtb_MessageBoxList.AppendText(messageText + "\n");
            }
        }

        private void ReceiveMessages()
        {
            while (client.Connected)
            {
                try
                {
                    if (stream.DataAvailable)
                    {
                        int bytesRead = stream.Read(protocol.Buffer, 0, protocol.Buffer.Length);
                        if (bytesRead > 0)
                        {
                            Payload payload = helper.ReplyBufferToPayload(protocol.Buffer);

                            if (payload.Type == TypePayload.MESSAGE)
                            {
                                try
                                {
                                    var json = ((JsonElement)payload.Data).GetRawText();
                                    var messageData = JsonSerializer.Deserialize<MessageData>(
                                        json,
                                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                                    );

                                    if (messageData != null)
                                    {
                                        string message = $"{messageData.From}: {messageData.Message}";
                                        if (!string.IsNullOrEmpty(messageData.To))
                                        {
                                            message += $" (para: {messageData.To})";
                                        }
                                        AddMessage(message);
                                    }
                                    else
                                    {
                                        AddMessage("Mensagem recebida, mas não pôde ser processada (null).");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    AddMessage($"Erro ao processar mensagem: {ex.Message}");
                                }
                            }
                            else if (payload.Type == TypePayload.FRIENDREQUEST)
                            {
                                // Processa a solicitação de amizade recebida
                                try
                                {
                                    var json = ((JsonElement)payload.Data).GetRawText();
                                    var friendRequestData = JsonSerializer.Deserialize<FriendRequestData>(
                                        json,
                                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                                    );
                                    if (friendRequestData != null)
                                    {
                                        string message = $"{friendRequestData.From} enviou uma solicitação de amizade.";
                                        AddMessage(message);
                                        // Aqui você pode implementar a lógica para aceitar ou rejeitar a solicitação
                                        // Por exemplo, abrir um diálogo para o usuário aceitar/rejeitar
                                        FriendRequest(
                                            friendRequestData.From,
                                            this.user.Username,
                                            false, // De início, não aceitamos a solicitação
                                            friendRequestData.SelfPublicKey
                                        );
                                    }
                                    else
                                    {
                                        AddMessage("Solicitação de amizade recebida, mas não pôde ser processada (null).");
                                    }
                                }

                                catch (Exception ex)
                                {
                                    AddMessage($"Erro ao processar solicitação de amizade: {ex.Message}");
                                }
                            }
                            if (payload.Type == TypePayload.FRIENDREPLY)
                            {
                                // Processa a resposta à solicitação de amizade
                                try
                                {
                                    var json = ((JsonElement)payload.Data).GetRawText();
                                    var friendReplyData = JsonSerializer.Deserialize<FriendReplyData>(
                                        json,
                                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                                    );
                                    if (friendReplyData != null)
                                    {
                                        string message = $"{friendReplyData.From} {(friendReplyData.Accepted ? "aceitou" : "rejeitou")} sua solicitação de amizade.";
                                        AddMessage(message);
                                        if (friendReplyData.Accepted)
                                        {
                                            // Aqui você pode adicionar lógica para adicionar o amigo à lista de amigos
                                        }
                                    }
                                    else
                                    {
                                        AddMessage("Resposta de amizade recebida, mas não pôde ser processada (null).");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    AddMessage($"Erro ao processar resposta de amizade: {ex.Message}");
                                }
                            }
                        }
                    }
                    Thread.Sleep(50);
                }
                catch
                {
                    AddMessage("Conexão encerrada.");
                    break;
                }
            }
        }

        /*
         FriendRequest -> Processa a solicitação de amizade recebida.
         FriendReply -> Processa a resposta à solicitação de amizade recebida.
         */


        public void FriendRequest(string targetUsername, string selfUsername, bool accepted, string selfPublicKey)
        {
            // Abre um diálogo com dois botões: Aceitar e Rejeitar
            DialogResult result = MessageBox.Show(
                $"{targetUsername} Recebeu uma solicitação de amizade. Aceitar?",
                "Solicitação de Amizade",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                // Se o usuário aceitar, envia uma resposta de aceitação
                var friendReplyData = new
                {
                    From = selfUsername,
                    To = targetUsername,
                    Accepted = true,
                    SelfPublicKey = selfPublicKey,
                    UniqueId = user.UniqueId
                };
                var payload = new Payload
                {
                    Type = TypePayload.FRIENDREPLY,
                    Data = friendReplyData,
                    Timestamp = DateTime.UtcNow.ToString("o")
                };
                byte[] packet = protocol.Make(ProtocolSICmdType.DATA, helper.PayloadToByte(payload));
                stream.Write(packet, 0, packet.Length);
                AddMessage($"Você aceitou a solicitação de amizade de {targetUsername}.");
            }
            else
            {
                // Se o usuário rejeitar, envia uma resposta de rejeição
                var friendReplyData = new
                {
                    From = selfUsername,
                    To = targetUsername,
                    Accepted = false,
                    SelfPublicKey = string.Empty // Chave pública vazia se rejeitado
                };
                var payload = new Payload
                {
                    Type = TypePayload.FRIENDREPLY,
                    Data = friendReplyData,
                    Timestamp = DateTime.UtcNow.ToString("o")
                };
                byte[] packet = protocol.Make(ProtocolSICmdType.DATA, helper.PayloadToByte(payload));
                stream.Write(packet, 0, packet.Length);
                AddMessage($"Você rejeitou a solicitação de amizade de {targetUsername}.");

            }
        }

        public void FriendReply(string targetUsername, string selfUsername, bool accepted, string selfPublicKey, string uniqueid)
        {
            // Processa a resposta à solicitação de amizade
            var friendReplyData = new
            {
                From = selfUsername,
                To = targetUsername,
                Accepted = accepted,
                SelfPublicKey = selfPublicKey
            };
            var payload = new Payload
            {
                Type = TypePayload.FRIENDREPLY,
                Data = friendReplyData,
                Timestamp = DateTime.UtcNow.ToString("o")
            };

            // Se o usuário aceitar, adiciona o amigo na db local
            if (accepted)
            {
                dbSetup.CreateFriend(
                    DbSetup.CreateConnection(),
                    selfUsername, // Usuário que está aceitando a solicitação
                    targetUsername, // Usuário que enviou a solicitação
                    selfPublicKey, // Chave pública do usuário que está aceitando
                    uniqueid // ID único da solicitação de amizade
                );
                AddMessage($"Você adicionou {targetUsername} como amigo.");
            }
            else
            {
                AddMessage($"Você rejeitou a solicitação de amizade de {targetUsername}.");
            }
        }

        private void FriendRequest(string targetUsername, string selfUsername, string selfPublickey, string uniqueid)
        {
            // Envia uma solicitação de amizade para o usuário especificado
            var friendRequestData = new
            {
                From = selfUsername,
                To = targetUsername,
                SelfPublicKey = selfPublickey,
                UniqueId = uniqueid
            };
            var payload = new Payload
            {
                Type = TypePayload.FRIENDREQUEST,
                Data = friendRequestData,
                Timestamp = DateTime.UtcNow.ToString("o")
            };
            byte[] packet = protocol.Make(ProtocolSICmdType.DATA, helper.PayloadToByte(payload));
            stream.Write(packet, 0, packet.Length);
            AddMessage($"Pedido de amizade enviado para {targetUsername}.");
        }


        public void ConnectToServer()
        {
            if (string.IsNullOrEmpty(this.user.Username)) return;

            try
            {
                client = new TcpClient();
                client.Connect(IPAddress.Loopback, 10000);
                stream = client.GetStream();

                var payload = new Payload
                {
                    Type = TypePayload.CONNECT,
                    Data = new { Username = this.user.Username, UniqueId = Guid.NewGuid().ToString() },
                    Timestamp = DateTime.UtcNow.ToString("o")
                };

                byte[] packet = protocol.Make(ProtocolSICmdType.DATA, helper.PayloadToByte(payload));
                stream.Write(packet, 0, packet.Length);
                stream.Read(protocol.Buffer, 0, protocol.Buffer.Length); // Espera ACK

                AddMessage("Conectado ao servidor.");

                Thread receiveThread = new Thread(ReceiveMessages);
                receiveThread.IsBackground = true;
                receiveThread.Start();
            }
            catch (Exception ex)
            {
                AddMessage($"Erro ao conectar: {ex.Message}");
            }
        }

        public void AddMessage(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(AddMessage), message);
                //dbSetup.CreateMessage(DbSetup.CreateConnection(), this.user.Username, toMessage.Text.Trim(), message);
            }
            else
            {
                rtb_MessageBoxList.AppendText($"{DateTime.Now.ToString("HH:mm:ss")} - {this.user.Username}: {message}\n");
            }
        }

        private void btn_SendMessage_Click(object sender, EventArgs e)
        {
            string messageText = textBox1.Text.Trim();
            if (string.IsNullOrEmpty(messageText)) return;
            var messageData = new MessageData
            {
                From = this.user.Username,
                Message = messageText,
                //To = toMessage.Text.Trim(),
            };
            var payload = new Payload
            {
                Type = TypePayload.MESSAGE,
                Data = messageData,
                Timestamp = DateTime.UtcNow.ToString("o")
            };
            byte[] packet = protocol.Make(ProtocolSICmdType.DATA, helper.PayloadToByte(payload));
            stream.Write(packet, 0, packet.Length);
            textBox1.Clear();
        }

        private void btn_FriendAdd_Click(object sender, EventArgs e)
        {
            // Abre um diálogo para adicionar amigos o dialog contém um TextBox para inserir o nome do amigo
            using (Form addFriendDialog = new Form())
            {
                addFriendDialog.Text = "Adicionar Amigo";
                TextBox friendNameTextBox = new TextBox { Dock = DockStyle.Fill };
                Button addButton = new Button { Text = "Adicionar", Dock = DockStyle.Bottom };
                addButton.Click += (s, args) =>
                {
                    string friendName = friendNameTextBox.Text.Trim();
                    if (!string.IsNullOrEmpty(friendName))
                    {
                        // Envia a solicitação de amizade
                        User user = dbSetup.GetUser(connection: DbSetup.CreateConnection(), this.user.Username);
                        FriendRequest(friendName, user.Username, user.GetPublicKey(), user.UniqueId);
                        AddMessage($"Pedido de amizade enviado para {friendName}.");
                        addFriendDialog.Close();
                    }
                    else
                    {
                        MessageBox.Show("Por favor, insira um nome de usuário válido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };
                addFriendDialog.Controls.Add(friendNameTextBox);
                addFriendDialog.Controls.Add(addButton);
                addFriendDialog.ShowDialog();
            }
        }

        private void lb_FriendList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Quando um amigo é selecionado na lista, limpa o chat e carrega as mensagens desse amigo.
            if (lb_FriendList.SelectedItem != null)
            {
                string selectedFriend = lb_FriendList.SelectedItem.ToString();
                rtb_MessageBoxList.Clear();
                LoadUserMsg(selectedFriend); // Carrega as mensagens do amigo selecionado
            }
        }

    }
}
