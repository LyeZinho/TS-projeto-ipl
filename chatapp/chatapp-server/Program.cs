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

    public static void Main()
    {
        Server server = new Server();
        server.Start();
    }

    public void Start()
    {
        TcpListener listener = new TcpListener(IPAddress.Any, PORT);
        listener.Start();
        Console.WriteLine("Servidor iniciado...");

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            Thread thread = new Thread(() => HandleClient(client));
            thread.Start();
        }
    }

    private void HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        ProtocolSI protocol = new ProtocolSI();

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
        User user = System.Text.Json.JsonSerializer.Deserialize<User>(payload.Data.ToString() ?? string.Empty)
            ?? throw new InvalidOperationException("Falha ao desserializar o usuário do payload de conexão.");
        connections.AddUser(user);
    }

    private void HandleMessage(Payload payload)
    {

        /*
         * Exemplo de payload de mensagem:
         {
            type: "MESSAGE", -> 
            data: {
              mesage: "Olá, mundo!", -> A mensagem que será enviada para o usuário e estara encriptada.
              from: "usuario1", -> O usuário que enviou a mensagem. (usa o de UniqueId)
              to: "usuario2" -> O usuário que receberá a mensagem. (usa o de UniqueId)
            },
            timestamp: "" -> Timestamp do comando, usado para sincronização e controle de tempo.
         }
         */
        // Processa o payload de mensagem
        if (payload == null || payload.Data == null)
            throw new ArgumentNullException(nameof(payload), "Payload de mensagem inválido.");
        
    }
}
