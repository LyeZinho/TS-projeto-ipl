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
        Payload payload = new Payload { Type = TypePayload.CONNECT };
        SerializationHelper helper = new SerializationHelper();

        byte[] packet = protocol.Make(ProtocolSICmdType.DATA, helper.PayloadToByte(payload));
        stream.Write(packet, 0, packet.Length);
        // Espera por ACK
        stream.Read(protocol.Buffer, 0, protocol.Buffer.Length);
        Console.WriteLine("Conexão estabelecida com o servidor.");
    }

    static void Main()
    {
        TcpClient client = new TcpClient();
        client.Connect(IPAddress.Loopback, PORT);

        NetworkStream stream = client.GetStream();
        ProtocolSI protocol = new ProtocolSI();

        Console.WriteLine("Digite mensagens (ou 'sair' para terminar):");
        string? input; // Permitir que a variável seja anulável

        // Envia solicitação de conexão
        Client clientInstance = new Client();
        clientInstance.SendConnectionRequest(stream, protocol);

        while (!string.IsNullOrEmpty(input = Console.ReadLine()) && input != "sair")
        {
            byte[] packet = protocol.Make(ProtocolSICmdType.DATA, input);
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
