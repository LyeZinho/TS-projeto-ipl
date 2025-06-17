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
using Microsoft.Data.Sqlite;

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
        private string SelectedFriend { get; set; } = string.Empty;

        public Chat(User user)
        {
            InitializeComponent();

            this.user = user.Username == null ? throw new ArgumentNullException(nameof(user)) : user; // Verifica se o usuário é nulo

            this.Text = $"Chat - {user.Username}"; // Define o título da janela com o nome do usuário

            protocol = new ProtocolSI();
            helper = new SerializationHelper();
            ConnectToServer();
            LoadChat(); // Carrega a lista de amigos do banco de dados local
        }

        private void LoadChat()
        {
            friendsList = dbSetup.GetFriends(DbSetup.CreateConnection(), user.Username);

            // Carrega a lista de amigos na ListBox (valida se tem amigos repetidos (não adiciona amigos repetidos))
            lb_FriendList.Items.Clear(); // Limpa a lista de amigos antes de carregar novos amigos
            foreach (var friend in friendsList)
            {
                if (!lb_FriendList.Items.Contains(friend.Username))
                {
                    lb_FriendList.Items.Add(friend.Username);
                }
            }
        }

        public string FriendPubKey(string friendUsername)
        {
            // Isto é um metodo alternativo (ja que o existente não funciona) Faz um pedido a api com endpoint getpublickey
            // endpoint: 'https://localhost:7016/User/getpublickey?username=<username>'
            /*
             Retorna:
            Response body

            {
              "publicKey": "MIIBCgKCAQEAsclH3e36agg82F8gbYlc30SvVRr5KHAUjz9TZ+6fDFen7mX7DUc7aOp67v4W+UnHvMUCkVM4+pO75mqESyym6xvUYWEFUDAhCbZ5ZfHPHLEkuwPdQKwtTU3u/0lzOsfOxOwRBkbkpUyhHPw+UfCgeSPJO5R/RvZaM+cFDYwHz3COwm441DkUdaFuM8BpSyvb/udvj9EecHuUoGWu9qJEAggk8shfRHWeWNlWfkJ2h83P8e9cCxnsaBC2S48FGNjBgMjP6q8gS1/Sup+9d/aXnRCJKlsFWHfwjwoMhHYUgqyiBimz3BK8yF3v+iAOYUv6eboHeWQzVcXEUtlFczCA1QIDAQAB"
            }
             */
            if (string.IsNullOrEmpty(friendUsername)) return string.Empty;
            try
            {
                // Faz uma requisição HTTP GET para obter a chave pública do amigo
                using (var client = new HttpClient())
                {
                    string url = $"https://localhost:7016/User/getpublickey?username={friendUsername}";
                    var response = client.GetAsync(url).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = response.Content.ReadAsStringAsync().Result;
                        var publicKeyData = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonResponse);
                        return publicKeyData["publicKey"];
                    }
                    else
                    {
                        AddMessage($"Erro ao obter a chave pública do amigo: {response.ReasonPhrase}");
                        return string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                AddMessage($"Erro ao obter a chave pública do amigo: {ex.Message}");
                return string.Empty;
            }
        }

        private void LoadUserMsg(string username)
        {
            // Carrega as mensagens do usuário especificado
            // Carrega as mensagens que foram enviadas para o usuário especificado
            // Merge as duas listas de mensagens evitando duplicar dados
            /*
             Explicação:
                As mensagens são organizadas por From e To, onde From é o remetente e To é o destinatário.
                porem o get messages pesquisa apenas por From, então é necessário fazer uma pesquisa por To também no caso de ser as mensagens enviadas pelo cliente atual.
             Assim, as mensagens são carregadas tanto as enviadas quanto as recebidas.
             */
            if (string.IsNullOrEmpty(username)) return;
            
            List<MessageDataClient> messagesFrom = dbSetup.GetMessages(DbSetup.CreateConnection(), username);
            List<MessageDataClient> messagesTo = dbSetup.GetMessages(DbSetup.CreateConnection(), this.user.Username);

            // Merge as duas listas de mensagens evitando duplicar dados
            var allMessages = messagesFrom.Concat(messagesTo)
                .GroupBy(m => new { m.From, m.To, m.MessageText, m.Timestamp })
                .Select(g => g.First())
                .OrderBy(m => m.Timestamp);

            rtb_MessageBoxList.Clear(); // Limpa o chat antes de carregar novas mensagens

            foreach (var message in allMessages)
            {
                string formattedMessage = $"{message.Timestamp:HH:mm:ss} - {message.From}: {message.MessageText}";
                if (!string.IsNullOrEmpty(message.To) && message.To != user.Username)
                {
                    formattedMessage += $" (para: {message.To})";
                }
                rtb_MessageBoxList.AppendText(formattedMessage + "\n");
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


                                        // Decifra a mensagem recebida usando a chave privada do usuário atual
                                        messageData.Message = Encript.DecryptMessage(
                                            messageData.Message,
                                            this.user.GetPrivateKey()
                                        );

                                        // Adiciona a mensagem ao banco de dados local
                                        dbSetup.CreateMessage(
                                            DbSetup.CreateConnection(),
                                            messageData.From,
                                            messageData.To,
                                            messageData.Message
                                        );
                                        string message = $"{messageData.From}: {messageData.Message}";
                                        if (!string.IsNullOrEmpty(messageData.To))
                                        {
                                            message += $" (para: {messageData.To})";
                                        }
                                        AddMessage(messageData.Message);
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
                                    // Deserializa o JSON da solicitação de amizade
                                    var json = ((JsonElement)payload.Data).GetRawText();

                                    // Tenta deserializar o JSON para um objeto FriendRequestData
                                    var friendRequestData = JsonSerializer.Deserialize<
                                        FriendRequestData>(
                                        json,
                                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                                    );

                                    // Se a deserialização for bem-sucedida, processa a solicitação de amizade
                                    if (friendRequestData != null)
                                    {
                                        string message = $"{friendRequestData.From} enviou uma solicitação de amizade.";
                                        AddMessage(message);

                                        // Processa a solicitação de amizade
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
                                    // Deserializa o JSON da resposta de amizade
                                    var json = ((JsonElement)payload.Data).GetRawText();
                                    var friendReplyData = JsonSerializer.Deserialize<FriendReplyData>(
                                        json,
                                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                                    );
                                    if (friendReplyData != null )
                                    {
                                        string message = $"{friendReplyData.From} {(friendReplyData.Accepted ? "aceitou" : "rejeitou")} sua solicitação de amizade.";
                                        AddMessage(message);
                                        if (friendReplyData.Accepted)
                                        {
                                            FriendReply(
                                                friendReplyData.To,
                                                friendReplyData.From,
                                                friendReplyData.Accepted,
                                                friendReplyData.SelfPublicKey,
                                                String.IsNullOrEmpty(friendReplyData.UniqueId) ? Guid.NewGuid().ToString() : friendReplyData.UniqueId
                                            );
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
                $"{selfUsername} Recebeu uma solicitação de amizade. de {targetUsername}. Aceitar?",
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

                // Adciona o amigo na db local 
                //public void CreateFriend(SqliteConnection connection, string username, string selfUsername, string uniqueId, string publicKey)
                dbSetup.CreateFriend(
                    DbSetup.CreateConnection(),
                    targetUsername, // Usuário que enviou a solicitação
                    selfUsername, // Usuário que está aceitando a solicitação
                    user.UniqueId, // ID único da solicitação de amizade
                    selfPublicKey // Chave pública do usuário que está aceitando
                );

                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => this.LoadChat()));
                }
                else
                {
                    this.LoadChat();
                }

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
                    targetUsername, // Usuário que enviou a solicitação
                    selfUsername, // Usuário que está aceitando a solicitação
                    uniqueid, // ID único da solicitação de amizade
                    selfPublicKey // Chave pública do usuário que está aceitando
                );
                AddMessage($"Você adicionou {targetUsername} como amigo.");

                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => this.LoadChat()));
                }
                else
                {
                    this.LoadChat();
                }
            }
            else
            {
                AddMessage($"Você rejeitou a solicitação de amizade de {targetUsername}.");
            }
        }

        private void SendFriendRequest(string targetUsername, string selfUsername, string selfPublickey, string uniqueid)
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
            }
            else
            {
                rtb_MessageBoxList.AppendText($"{DateTime.Now.ToString("HH:mm:ss")} - {this.user.Username}: {message}\n");
            }
        }

        private void btn_SendMessage_Click(object sender, EventArgs e)
        {
            string messageTextRaw = textBox1.Text.Trim();
            if (string.IsNullOrEmpty(messageTextRaw)) return;

            // Encripta a mensagem antes de enviá-la

            //1. Busca o amigo selecionado na lista de amigos
            if (string.IsNullOrEmpty(SelectedFriend))
            {
                MessageBox.Show("Por favor, selecione um amigo para enviar a mensagem.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            Friend friend = friendsList.FirstOrDefault(f => f.Username == SelectedFriend);

            if (friend == null)
            {
                MessageBox.Show("Amigo selecionado não encontrado.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Adciona o prefixo do usuário para identificar quem enviou a mensagem ex: (username) Mensagem
            messageTextRaw = $"({this.user.Username}) {messageTextRaw}";

            //2. Encripta a mensagem usando a chave pública do amigo selecionado
            string messageText = Encript.EncryptMessage(messageTextRaw, FriendPubKey(friend.Username));

            var messageData = new MessageData
            {
                From = this.user.Username,
                Message = messageText,
                To = SelectedFriend,
            };
            var payload = new Payload
            {
                Type = TypePayload.MESSAGE,
                Data = messageData,
                Timestamp = DateTime.UtcNow.ToString("o")
            };

            // Adiciona a mensagem ao banco de dados local
            dbSetup.CreateMessage(
                DbSetup.CreateConnection(), 
                this.user.Username, 
                SelectedFriend, 
                messageTextRaw
                );

            // Adciona a mensagem ao historico
            AddMessage(messageTextRaw);

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
                        // private void SendFriendRequest(string targetUsername, string selfUsername, string selfPublickey, string uniqueid)
                        User user = dbSetup.GetUser(connection: DbSetup.CreateConnection(), this.user.Username);
                        SendFriendRequest(
                            friendName, 
                            user.Username, 
                            user.GetPublicKey(), 
                            user.UniqueId
                        );
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
                //string selectedFriend = lb_FriendList.SelectedItem.ToString();
                this.SelectedFriend = lb_FriendList.SelectedItem.ToString();
                rtb_MessageBoxList.Clear();
                LoadUserMsg(this.SelectedFriend);
            }
        }
    }
}
