using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using EI.SI;
using chatlib;
using System.Text.Json;


class Server
{
    private const int PORT = 10000;
    private Connections connections = new Connections();
    private SerializationHelper helper = new SerializationHelper();
    private ProtocolSI protocol = new ProtocolSI();

    public static void Main()
    {
        new Server().Start();
    }

    public void Start()
    {
        var listener = new TcpListener(IPAddress.Any, PORT);
        listener.Start();
        Console.WriteLine("Servidor iniciado...");

        while (true)
        {
            var client = listener.AcceptTcpClient();
            Console.WriteLine("Cliente conectado: " + client.Client.RemoteEndPoint);
            var thread = new Thread(() => HandleClient(client));
            thread.Start();
        }
    }

    private void HandleClient(TcpClient client)
    {
        var stream = client.GetStream();
        try
        {
            while (client.Connected)
            {
                // aguarda dados
                int bytesRead = stream.Read(protocol.Buffer, 0, protocol.Buffer.Length);
                if (bytesRead == 0)
                {
                    Console.WriteLine("Conexão fechada pelo cliente.");
                    break;
                }

                if (protocol.GetCmdType() == ProtocolSICmdType.DATA)
                {
                    Console.WriteLine("Recebido: " + protocol.GetStringFromData());
                    var payload = helper.ByteToPayload(protocol.GetData());
                    try
                    {
                        ProcessPayload(payload, client, stream);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Erro ao processar payload: " + ex.Message);
                    }
                }
                else if (protocol.GetCmdType() == ProtocolSICmdType.EOT)
                {
                    var ack = protocol.Make(ProtocolSICmdType.ACK);
                    stream.Write(ack, 0, ack.Length);
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro no cliente: " + ex.Message);
        }
        finally
        {
            stream.Close();
            client.Close();
        }
    }

    // Agora recebe também o TcpClient e o NetworkStream
    public void ProcessPayload(Payload payload, TcpClient client, NetworkStream stream)
    {
        if (payload == null)
            throw new ArgumentNullException(nameof(payload));

        switch (payload.Type)
        {
            case TypePayload.CONNECT:
                HandleConnect(payload, client, stream);
                break;
            case TypePayload.MESSAGE:
                HandleMessage(payload);
                break;
            case TypePayload.FRIENDREQUEST:
                Console.WriteLine("Solicitação de amizade recebida: " + payload.Data);
                HandleFriendRequest(payload);
                break;
            case TypePayload.FRIENDREPLY:
                Console.WriteLine("Resposta à solicitação de amizade recebida: " + payload.Data);
                HandleFriendReply(payload);
                break;
            case TypePayload.ACK:
                Console.WriteLine("ACK recebido: " + payload.Data);
                break;
            case TypePayload.EOT:
                Console.WriteLine("Fim de transmissão recebido.");
                break;

        }
    }

    // Refatorado para receber TcpClient e Stream
    private void HandleConnect(Payload payload, TcpClient client, NetworkStream stream)
    {
        if (payload.Data == null)
            throw new ArgumentNullException(nameof(payload), "Payload de conexão inválido.");

        var data = JsonSerializer.Deserialize<ConnectionData>(payload.Data.ToString() ?? string.Empty)
                   ?? throw new InvalidOperationException("Falha ao desserializar dados de conexão.");

        // Cria o User com o TcpClient já associado
        var user = new User()
            .SetUsername(data.Username)
            .SetUniqueId(data.UniqueId)
            .SetClient(client)
            .Build();

        connections.AddUser(user);

        // Envia ACK
        var response = new Payload
        {
            Type = TypePayload.ACK,
            Data = "Conexão estabelecida com sucesso.",
            Timestamp = DateTime.UtcNow.ToString("o")
        };
        var respBytes = helper.PayloadToByte(response);

        if (stream.CanWrite)
        {
            stream.Write(respBytes, 0, respBytes.Length);
            Console.WriteLine($"[INFO] Usuário {user.Username} ({user.UniqueId}) conectado.");
        }
        else
        {
            Console.WriteLine($"[ERRO] Não foi possível escrever no stream de {user.UniqueId}.");
        }
    }

    private void HandleMessage(Payload payload)
    {
        var msg = JsonSerializer.Deserialize<MessageData>(payload.Data.ToString() ?? string.Empty)
                  ?? throw new InvalidOperationException("Falha ao desserializar dados de mensagem.");

        var sender = connections.GetUserByUsername(msg.From);
        var recipient = connections.GetUserByUsername(msg.To);

        if (sender == null || recipient == null)
        {
            Console.WriteLine("Remetente ou destinatário não encontrado.");
            return;
        }

        var response = new Payload
        {
            Type = TypePayload.MESSAGE,
            Data = new { message = msg.Message, from = msg.From, to = msg.To },
            Timestamp = DateTime.UtcNow.ToString("o")
        };
        var respBytes = helper.PayloadToByte(response);

        var tcp = recipient.GetClient();
        if (tcp.Connected)
        {
            var rs = tcp.GetStream();
            if (rs.CanWrite)
            {
                rs.Write(respBytes, 0, respBytes.Length);
                Console.WriteLine($"Mensagem enviada para {recipient.Username}.");
            }
        }
    }




    private void HandleFriendRequest(Payload payload)
    {
        // Implementar lógica de solicitação de amizade

        /*
         Como vai funcionar:

           1. O usuário envia uma solicitação de amizade com o nome do amigo e sua chave pública.
           2. O servidor recebe a solicitação e verifica se o usuário existe.
           3. Se o usuário existir, o servidor envia um payload de solicitação de amizade para o usuário. (contendo a chave pública do remetente)
           4. O usuário recebe a solicitação e pode aceitar ou rejeitar.
           5. Se aceitar, o servidor adiciona o usuário à lista de amigos e envia um payload de confirmação. (contendo a chave pública do recipiente)
         */

        if (payload.Data == null)
            throw new ArgumentNullException(nameof(payload), "Payload de solicitação de amizade inválido.");
        var data = JsonSerializer.Deserialize<FriendRequestData>(payload.Data.ToString() ?? string.Empty)
                   ?? throw new InvalidOperationException("Falha ao desserializar dados de solicitação de amizade.");
        var sender = connections.GetUserByUsername(data.From);
        var recipient = connections.GetUserByUsername(data.To);
        if (sender == null || recipient == null)
        {
            Console.WriteLine("Remetente ou destinatário não encontrado.");
            return;
        }
        // Envia a solicitação de amizade para o destinatário
        var response = new Payload
        {
            Type = TypePayload.FRIENDREQUEST,
            Data = new
            {
                from = data.From,
                to = data.To,
                selfPublicKey = data.SelfPublicKey
            },
            Timestamp = DateTime.UtcNow.ToString("o")
        };
        var respBytes = helper.PayloadToByte(response);
        var tcp = recipient.GetClient();
        if (tcp.Connected)
        {
            var rs = tcp.GetStream();
            if (rs.CanWrite)
            {
                rs.Write(respBytes, 0, respBytes.Length);
                Console.WriteLine($"Solicitação de amizade enviada de {data.From} para {data.To}.");
            }
            else
            {
                Console.WriteLine($"[ERRO] Não foi possível escrever no stream de {recipient.Username}.");
            }
        }
        else
        {
            Console.WriteLine($"[ERRO] Cliente {recipient.Username} não está conectado.");
        }
    }

    private void HandleFriendReply(Payload payload)
    {
        // Implementar lógica de resposta à solicitação de amizade
        if (payload.Data == null)
            throw new ArgumentNullException(nameof(payload), "Payload de resposta à solicitação de amizade inválido.");

        var data = JsonSerializer.Deserialize<FriendReplyData>(payload.Data.ToString() ?? string.Empty)
                   ?? throw new InvalidOperationException("Falha ao desserializar dados de resposta à solicitação de amizade.");

        var sender = connections.GetUserByUsername(data.From);
        var recipient = connections.GetUserByUsername(data.To);

        if (sender == null || recipient == null)
        {
            Console.WriteLine("Remetente ou destinatário não encontrado.");
            return;
        }
        if (data.Accepted)
        {
            // Adiciona o remetente à lista de amigos do destinatário
            connections.AddFriend(recipient.Username, sender.Username);
            Console.WriteLine($"Amizade aceita entre {data.From} e {data.To}.");
        }
        else
        {
            Console.WriteLine($"Amizade rejeitada entre {data.From} e {data.To}.");
        }
        // Envia a resposta ao remetente
        var response = new Payload
        {
            Type = TypePayload.FRIENDREPLY,
            Data = new
            {
                from = data.From,
                to = data.To,
                accepted = data.Accepted,
                selfPublicKey = data.SelfPublicKey
            },
            Timestamp = DateTime.UtcNow.ToString("o")
        };

        var respBytes = helper.PayloadToByte(response);

        var tcp = sender.GetClient();
        if (tcp.Connected)
        {
            var rs = tcp.GetStream();
            if (rs.CanWrite)
            {
                rs.Write(respBytes, 0, respBytes.Length);
                Console.WriteLine($"Resposta à solicitação de amizade enviada para {data.From}.");
            }
            else
            {
                Console.WriteLine($"[ERRO] Não foi possível escrever no stream de {sender.Username}.");
            }
        }
    }

}
