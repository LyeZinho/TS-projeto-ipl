using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using EI.SI;
using chatlib;


class Server
{
    private const int PORT = 10000;
    private readonly Connections connections = new Connections();
    SerializationHelper helper = new SerializationHelper();
    private object client;
    NetworkStream stream = null!; // Inicializado como null, será atribuído no HandleClient
    ProtocolSI protocol = new ProtocolSI();

    // Lista de conexões ativas (não usada para conexões de users e sim para sockets)
     private List<TcpClient> activeConnections = new List<TcpClient>();

    public static void Main()
    {
        Server server = new Server();
        server.Start();
    }

    // Tipos para deserialização/serialização de payloads
    private class MessageData
    {
        public string Message { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }

    class ConnectionData
    {
        public string Username { get; set; }
        public string UniqueId { get; set; }
    }

    public void Start()
    {
        TcpListener listener = new TcpListener(IPAddress.Any, PORT);
        listener.Start();
        Console.WriteLine("Servidor iniciado...");

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("Cliente conectado: " + client.Client.RemoteEndPoint);
            activeConnections.Add(client); // Adiciona o cliente à lista de conexões ativas
            Thread thread = new Thread(() => HandleClient(client));
            thread.Start();
        }
    }

    // SERVIDOR
    private void HandleClient(TcpClient client)
    {
        // Método para lidar com a conexão do cliente
        stream = client.GetStream();

        // Lê o protocolo de conexão
        while (protocol.GetCmdType() != ProtocolSICmdType.EOT)
        {
            stream.Read(protocol.Buffer, 0, protocol.Buffer.Length);

            switch (protocol.GetCmdType())
            {
                case ProtocolSICmdType.DATA:
                    Console.WriteLine("Recebido: " + protocol.GetStringFromData());
                    Payload payload = new SerializationHelper().ByteToPayload(protocol.GetData());
                    Console.WriteLine("Payload: " + payload.Type);
                    try
                    {
                        ProcessPayload(payload); // <- método de instância acessando estado interno
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Erro: " + ex.Message);
                    }
                    stream.Write(protocol.Make(ProtocolSICmdType.ACK), 0, protocol.Make(ProtocolSICmdType.ACK).Length);
                    break;

                case ProtocolSICmdType.EOT:
                    stream.Write(protocol.Make(ProtocolSICmdType.ACK), 0, protocol.Make(ProtocolSICmdType.ACK).Length);
                    break;
            }
        }

        stream.Close();
        client.Close();
    }

    public void ProcessPayload(Payload payload)
    {
        if (payload == null) 
            throw new ArgumentNullException(nameof(payload));
        switch (payload.Type)
        {
            case TypePayload.CONNECT:
                HandleConnect(payload);
                break;
            case TypePayload.MESSAGE:
                HandleMessage(payload);
                break;
            case TypePayload.FRIENDADD:
                // Aqui você pode implementar a lógica para adicionar amigos
                throw new NotImplementedException("Lógica de adição de amigos ainda não implementada.");
            case TypePayload.FRIENDREMOVE:
                // Aqui você pode implementar a lógica para remover amigos
                throw new NotImplementedException("Lógica de remoção de amigos ainda não implementada.");
            case TypePayload.LOGIN:
                // Aqui você pode implementar a lógica de login
                throw new NotImplementedException("Lógica de login ainda não implementada.");
            case TypePayload.ACK:
                // Aqui você pode implementar a lógica de confirmação
                Console.WriteLine("ACK recebido: " + payload.Data);
                break;
            case TypePayload.EOT:
                // Aqui você pode implementar a lógica de fim de transmissão
                Console.WriteLine("Fim de transmissão recebido.");
                break;

        }
    }
    
    

    private void HandleConnect(Payload payload)
    {
        // Processa o payload de conexão
        if (payload == null || payload.Data == null)
            throw new ArgumentNullException(nameof(payload), "Payload de conexão inválido.");
        // Converte o payload.Data para o tipo esperado (dynamic ou um tipo específico)
        /*
         Servidor iniciado...
        Recebido: {"Type":0,"Data":{"Username":"usuario1","UniqueId":"f66f1390-5b2a-49c2-b4ca-2cb3df688d44"},"Timestamp":"2025-06-03T18:51:33.4064394Z"}
        Payload: CONNECT
        ValueKind = Object : "{"Username":"usuario1","UniqueId":"8dfa5532-1847-46c4-881e-0ab7d96c91a7"}" <- Deserializa o valor data dentro do payload
        */
        var connectionData = System.Text.Json.JsonSerializer.Deserialize<ConnectionData>(payload.Data.ToString() ?? string.Empty)
            ?? throw new InvalidOperationException("Falha ao desserializar os dados de conexão.");


        ProtocolSI protocol = new ProtocolSI();
        // Sends an ACK response to the client

        Payload responsePayload = new Payload
        {
            Type = TypePayload.ACK,
            Data = "Conexão estabelecida com sucesso.",
            Timestamp = DateTime.UtcNow.ToString("o") // ISO 8601 format
        };

        // method to wrie back: byte[] packet = protocol.Make(ProtocolSICmdType.DATA, helper.PayloadToByte(payload));
        byte[] responseData = new SerializationHelper().PayloadToByte(responsePayload);
        byte[] packet = protocol.Make(ProtocolSICmdType.DATA, responseData);

        User user = new User()
            .SetUsername(connectionData.Username)
            .SetUniqueId(connectionData.UniqueId)
            .Build();

        connections.AddUser(user); // Adiciona o usuário à lista de conexões

        // Obtem o cliente conectado e envia o pacote de dados
        if (stream != null && stream.CanWrite)
        {
            stream.Write(packet, 0, packet.Length);
            Console.WriteLine($"Conexão estabelecida com o usuário: {user.Username} (ID: {user.UniqueId})");
        }
        else
        {
            Console.WriteLine("Erro ao enviar resposta de conexão. O fluxo não está disponível.");
        }
    }

    private void HandleMessage(Payload payload)
    {
        /*
         * Exemplo de payload de mensagem:
         {
            type: "MESSAGE", -> 
            data: {
              message: "Olá, mundo!", -> A mensagem que será enviada para o usuário e estará encriptada.
              from: "usuario1", -> O usuário que enviou a mensagem. (usa o de UniqueId)
              to: "usuario2" -> O usuário que receberá a mensagem. (usa o de UniqueId)
            },
            timestamp: "" -> Timestamp do comando, usado para sincronização e controle de tempo.
         }
         */
        // Processa o payload de mensagem
        if (payload == null || payload.Data == null)
            throw new ArgumentNullException(nameof(payload), "Payload de mensagem inválido.");

        // Converte o payload.Data para o tipo esperado (dynamic ou um tipo específico)
        var messageData = System.Text.Json.JsonSerializer.Deserialize<MessageData>(payload.Data.ToString() ?? string.Empty)
            ?? throw new InvalidOperationException("Falha ao desserializar os dados da mensagem.");

        // Get all tcp clients connected (not on connections i want to get all sockets)
        foreach (var user in connections.GetAllUsers())
        {
            Console.WriteLine($"Enviando mensagem para {user.Username}...");
        }


        Console.WriteLine("Mensagem recebida: " + messageData.Message);

        // Resposta de teste // Basicamente enviando a mensagem de volta para o usuário que enviou
        Payload responsePayload = new Payload
        {
            Type = TypePayload.MESSAGE,
            Data = new
            {
                message = messageData.Message,
                from = messageData.From,
                to = messageData.To
            },
            Timestamp = DateTime.UtcNow.ToString("o") // Formato ISO 8601
        };

        byte[] responseData = new SerializationHelper().PayloadToByte(responsePayload);

        byte[] packet = protocol.Make(ProtocolSICmdType.DATA, responseData);

        // Envia o pacote de dados para o usuário que enviou a mensagem
        if (stream != null && stream.CanWrite)
        {
            stream.Write(packet, 0, packet.Length);
            Console.WriteLine($"Mensagem enviada para {messageData.To} de {messageData.From}: {messageData.Message}");
        }
        else
        {
            Console.WriteLine("Erro ao enviar mensagem. O fluxo não está disponível.");
        }
    }

    private void HandleRegister(Payload payload)
    {
        // Processa o payload de registro
        /*
         
         */

    }

    
}
