// Arquivo: chatlib/client/ChatClient.cs
using System;
using System.Text;
using System.Text.Json;
using chatlib.objects;

// WS server e client
using ProtoIP;

namespace chatlib.client
{

    // CLIENTE (posteriormente vai ser movido para o projeto do cliente)
    public class ChatClient : ProtoClient
    {
        private string _username;   // Nome de usuário
        private string _publicKey;  //
        private string _privateKey; // Chave privada

        public event Action<string, string>? MessageReceived; // Evento para quando uma mensagem é recebida
        public event Action<string, string>? FriendAdded; // Evento para quando um amigo é adicionado

        // <string, string> = <username, publicKey>
        public Dictionary<string, string> Friends { get; set; } = new Dictionary<string, string>(); // Lista de amigos

        public ChatClient(string username, int localPort, string publicKey, string privateKey)
        {
            _username = username;
            _publicKey = publicKey;
            _privateKey = privateKey;
        }

        public void Start()
        {
            // Inicia a conexão com o servidor
            Start();
        }

        public void Stop()
        {
            // Encerra a conexão com o servidor
            Stop();
        }

        public byte[] StringToByteArr(string str)
        {
            // Converte uma string para um array de bytes
            return Encoding.UTF8.GetBytes(str);
        }

        public void SendMessage(string targetUsername, string message)
        {
            // Envia uma mensagem para o usuário de destino
            var payload = new
            {
                messagetype = "message",
                sender = _username,
                target = targetUsername,
                publicKey = _publicKey,
                message,
                timestamp = DateTime.UtcNow.ToString("o"),
            };
            Send(StringToByteArr(JsonSerializer.Serialize(payload)));
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
            Send(StringToByteArr(JsonSerializer.Serialize(payload)));

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
    }
}
