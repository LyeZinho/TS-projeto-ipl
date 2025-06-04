using System;
using System.Net;
using System.Net.Sockets;
using EI.SI;
using chatlib;
using System.Runtime.CompilerServices;

class Client
{
    private const int PORT = 10000;

    public void SendConnectionRequest(NetworkStream stream, ProtocolSI protocol)
    {
        // Envia um payload de conexão
        Payload payload = new Payload { Type = TypePayload.CONNECT,
        Data = new
        {
            Username = "usuario1", // Nome do usuário que está se conectando
            UniqueId = Guid.NewGuid().ToString() // ID único para o usuário
        },
            Timestamp = DateTime.UtcNow.ToString("o") // Formato ISO 8601
        };
        SerializationHelper helper = new SerializationHelper();

        byte[] packet = protocol.Make(ProtocolSICmdType.DATA, helper.PayloadToByte(payload));
        stream.Write(packet, 0, packet.Length);
        // Espera por ACK
        stream.Read(protocol.Buffer, 0, protocol.Buffer.Length);
        Console.WriteLine("Conexão estabelecida com o servidor.");
    }

    public string ByteArrToString(byte[] input )
    {
        return System.Text.Encoding.UTF8.GetString(input).TrimEnd('\0');
    }

    // CLIENTE
    static void Main()
    {
        TcpClient client = new TcpClient();
        client.Connect(IPAddress.Loopback, PORT);
        NetworkStream stream = client.GetStream();
        ProtocolSI protocol = new ProtocolSI();
        SerializationHelper helper = new SerializationHelper();

        Console.WriteLine("Digite mensagens (ou 'sair' para terminar):");
        string? input;

        Client clientInstance = new Client();
        //clientInstance.SendConnectionRequest(stream, protocol);

        // Thread para receber mensagens do servidor
        var receiveThread = new Thread(() =>
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
                            Console.WriteLine("Dados recebidos do servidor: " +
                                System.Text.Encoding.UTF8.GetString(protocol.Buffer, 0, bytesRead));

                            try
                            {
                                // Deserializa o payload recebido primeiro para string (motivo de teste)
                                string receivedData = clientInstance.ByteArrToString(protocol.Buffer);
                                Payload payload = helper.ReplyBufferToPayload(protocol.Buffer);

                                if (payload.Type == TypePayload.MESSAGE)
                                {
                                    Console.WriteLine($"Mensagem recebida de {payload.Data}: {payload.Data}");
                                }
                                else if (payload.Type == TypePayload.ACK)
                                {
                                    Console.WriteLine("ACK recebido do servidor.");
                                }
   
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Erro ao processar payload: " + ex.Message);
                            }
                        }
                    }
                    Thread.Sleep(50); // Evita uso excessivo de CPU
                }
                catch
                {
                    break;
                }
            }
        });


        receiveThread.IsBackground = true;
        receiveThread.Start();

        // Loop principal para enviar mensagens
        while (!string.IsNullOrEmpty(input = Console.ReadLine()) && input != "sair")
        {
            Payload payload = new Payload
            {
                Type = TypePayload.MESSAGE,
                Data = new
                {
                    Message = input,
                    From = "usuario1",
                    To = "usuario2"
                },
                Timestamp = DateTime.UtcNow.ToString("o")
            };

            byte[] packet = protocol.Make(ProtocolSICmdType.DATA, helper.PayloadToByte(payload));
            stream.Write(packet, 0, packet.Length);

            // Espera por ACK
            stream.Read(protocol.Buffer, 0, protocol.Buffer.Length);
        }

        // Envia EOT
        stream.Write(protocol.Make(ProtocolSICmdType.EOT), 0, protocol.Make(ProtocolSICmdType.ACK).Length);
        stream.Read(protocol.Buffer, 0, protocol.Buffer.Length);

        stream.Close();
        client.Close();
    }
}
