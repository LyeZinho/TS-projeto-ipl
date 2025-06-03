// Arquivo: chatlib/client/ChatClient.cs
using System;
using System.Text;
using System.Text.Json;
using chatlib.objects;

// WS server e client+
using ProtoIP;

namespace chatlib.client
{
    // Classe que representa o cliente de chat (herda de ProtoClient)
    public class ChatClient : ProtoClient
    {
        private string _username;   // Nome de usuário
        private string _publicKey;  // Chave pública
        private string _privateKey; // Chave privada
        private string _serverIp; // IP do servidor
        private int _serverPort; // Porta do servidor
        RsaChatCrypto _rsaChatCrypto = new RsaChatCrypto(); // Instância da classe de criptografia

        public event Action<string, string>? MessageReceived; // Evento para quando uma mensagem é recebida
        public event Action<string, string>? FriendAdded; // Evento para quando um amigo é adicionado

        // <string, string> = <username, publicKey>
        public Dictionary<string, string> Friends { get; set; } = new Dictionary<string, string>(); // Lista de amigos

        public ChatClient(string username, string publicKey, string privateKey, string serverip, int serverport)
        {
            _username = username;
            _publicKey = publicKey;
            _privateKey = privateKey;
            _serverIp = serverip;
            _serverPort = serverport;
        }

        public void ConnectServer()
        {
            // Inicia a conexão com o servidor
            this.Connect(_serverIp, _serverPort);
        }

        public byte[] StringToByteArr(string str)
        {
            // Converte uma string para um array de bytes
            return Encoding.UTF8.GetBytes(str);
        }

        public void SendMessage(string targetUsername, string message, string targetPubKey)
        {
            // Criptografa a mensagem com a chave pública do destinatário
            byte[] encryptedMessage = RsaChatCrypto.Encriptar(message, Convert.FromBase64String(targetPubKey));
            string encryptedMessageBase64 = Convert.ToBase64String(encryptedMessage);

            // Cria o payload da mensagem
            var payload = new
            {
                messagetype = "message",
                sender = _username,
                target = targetUsername,
                publicKey = _publicKey,
                message = encryptedMessageBase64,
                timestamp = DateTime.UtcNow.ToString("o"),
            };

            string json = JsonSerializer.Serialize(payload);

            this.Send(StringToByteArr(json));
        }

        public void AddFriend(string friendUsername, string friendPublicKey)
        {
            // Envia um pedido de amizade para o usuário de destino
            var payload = new
            {
                messagetype = "friend-add",
                sender = _username,
                target = friendUsername,
                publicKey = _publicKey,
                friendUsername,
                friendPublicKey,
                timestamp = DateTime.UtcNow.ToString("o"),
            };

            // Envia o pedido de amizade para o servidor
            this.Send(StringToByteArr(JsonSerializer.Serialize(payload)));

            // Aguarda a resposta do servidor
            var response = AssembleReceivedDataIntoPacket();
            var responseData = response.GetDataAs<string>();
            var responseDict = JsonSerializer.Deserialize<Dictionary<string, object>>(responseData);
            var responseMessageTypeStr = responseDict["messagetype"].ToString() ?? string.Empty;
            var responseSender = responseDict["sender"].ToString() ?? string.Empty;
            var responseTarget = responseDict["target"].ToString() ?? string.Empty;
            var responsePublicKey = responseDict["publicKey"].ToString() ?? string.Empty;
            var responseMessage = responseDict["message"].ToString() ?? string.Empty;
            var responseTimestamp = responseDict["timestamp"].ToString() ?? string.Empty;
            var responseDataJson = responseDict["data"].ToString() ?? string.Empty;
            var responseDataObj = JsonSerializer.Deserialize<Dictionary<string, object>>(responseDataJson);
            if (
                string.IsNullOrEmpty(responseMessageTypeStr)
                || string.IsNullOrEmpty(responseSender)
                || string.IsNullOrEmpty(responseTarget)
                || string.IsNullOrEmpty(responsePublicKey)
                || string.IsNullOrEmpty(responseMessage)
                || string.IsNullOrEmpty(responseTimestamp)
            )
            {
                throw new Exception("Recebido pacote inválido");
            }

            // Verifica o tipo do pacote e executa a ação correspondente
            switch (responseMessageTypeStr)
            {
                case "friend-add":
                    // Adiciona o amigo
                    FriendAdded?.Invoke(responseSender, responseMessage);
                    break;
                default:
                    throw new Exception("Tipo de pacote desconhecido");
            }
        }

        // This should be part of an Threading or Task for can be used on frontend
        public Task<string> MessageGet(string message)
        {
            // Aguarda a resposta do servidor
            var response = AssembleReceivedDataIntoPacket();

            // Deserializa a resposta do servidor
            var responseData = response.GetDataAs<string>();
            var responseDict = JsonSerializer.Deserialize<Dictionary<string, object>>(responseData);
            var responseMessageTypeStr = responseDict["messagetype"].ToString() ?? string.Empty;
            var responseSender = responseDict["sender"].ToString() ?? string.Empty;
            var responseTarget = responseDict["target"].ToString() ?? string.Empty;
            var responsePublicKey = responseDict["publicKey"].ToString() ?? string.Empty;
            var responseMessage = responseDict["message"].ToString() ?? string.Empty;
            var responseTimestamp = responseDict["timestamp"].ToString() ?? string.Empty;
            var responseDataJson = responseDict["data"].ToString() ?? string.Empty;
            var responseDataObj = JsonSerializer.Deserialize<Dictionary<string, object>>(responseDataJson);
            if (
                string.IsNullOrEmpty(responseMessageTypeStr)
                || string.IsNullOrEmpty(responseSender)
                || string.IsNullOrEmpty(responseTarget)
                || string.IsNullOrEmpty(responsePublicKey)
                || string.IsNullOrEmpty(responseMessage)
                || string.IsNullOrEmpty(responseTimestamp)
            )
            {
                throw new Exception("Recebido pacote inválido");
            }
            // Verifica o tipo do pacote e executa a ação correspondente
            switch (responseMessageTypeStr)
            {
                case "message":
                    // Descriptografa a mensagem com a chave privada do destinatário
                    string messageRecived = RsaChatCrypto.Desencriptar(Convert.FromBase64String(responseMessage), Convert.FromBase64String(_privateKey));
                    MessageReceived?.Invoke(responseSender, message);
                    return Task.FromResult(messageRecived);
                case "friend-add":
                    // Adiciona o amigo
                    FriendAdded?.Invoke(responseSender, responseMessage);
                    return Task.FromResult(responseMessage);
                default:
                    throw new Exception("Tipo de pacote desconhecido");
            }
        }
    }
}