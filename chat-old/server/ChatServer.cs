using System;
using System.Linq;
using System.Text;
using chatlib.objects;
using System.Text.Json;






/*
 * EXEMPLOS DADOS PELA BIBLIOTECA ProtoIP
 * de (JoaoAJMatos) https://github.com/JoaoAJMatos/ProtoIP
 * 
protoIP examples:

using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using ProtoIP;

class ComplexClient : ProtoClient
{
      // We can override the OnSend() method to describe our Client logic
      // In this example, we are printing out a message when the server responds
      public override void OnReceive() 
      {
            Packet receivedPacket = AssembleReceivedDataIntoPacket();

            // We can check the type of the packet and act accordingly
            if (receivedPacket._GetType() == (int)Packet.Type.PONG)
            {
                  Console.WriteLine("CLIENT: Received PONG packet!");
            }
      }
}

class ComplexServer : ProtoServer
{
      // We can override the OnRequest() method to describe our Server logic
      // Once the server receives a request, it will call this method
      public override void OnRequest(int userID)
      {
            Packet receivedPacket = AssembleReceivedDataIntoPacket(userID);

            // We can check the type of the packet and act accordingly
            if (receivedPacket._GetType() == (int)Packet.Type.PING)
            {
                  Console.WriteLine("SERVER: Received PING packet, sending PONG!");
                  Packet packet = new Packet(Packet.Type.PONG);
                  Send(Packet.Serialize(packet), userID);
            }
      }
}

class Program
{
      static void Main()
      {
            int PORT = ProtoIP.Common.Network.GetRandomUnusedPort();

            // Create the server and start it
            ComplexServer server = new ComplexServer();
            Thread serverThread = new Thread(() => server.Start(PORT));
            serverThread.Start();

            // Create a new ComplexClient object and connect to the server
            ComplexClient client = new ComplexClient();
            client.Connect("127.0.0.1", PORT);

            // Serialize the packet and send it
            Packet packet = new Packet(Packet.Type.PING);
            client.Send(Packet.Serialize(packet));

            // Receive the response from the server
            client.Receive();

            // Disconnect from the server
            client.Disconnect();

            // Stop the server
            server.Stop();
      }
}

---------------------

// Copyright (c) 2023, João Matos
// Check the end of the file for extended copyright notice.

using System;
using System.Net.Sockets;
using System.Text;

// Include the ProtoIP namespace
using ProtoIP;

class ComplexClient : ProtoClient 
{
      // We can override the OnConnect() method to describe our Client logic
      // In this example, we are printing out the message we get back from the
      // remote host.
      public override void OnReceive() 
      {
            string data = _protoStream.GetDataAs<string>();
            Console.WriteLine(data);
      }
}

class Program 
{
      static void Main() 
      {
            // Create a new ComplexClient object and connect to the server
            ComplexClient client = new ComplexClient();
            client.Connect("1.1.1.1", 1234);

            // Send data to the server
            client.Send("Hello World!");

            // Receive the response
            // The OnReceive() method will be called
            client.Receive();

            // Disconnect from the server
            client.Disconnect();
      }
}

----------------------
exampleComplexServer
using System;
using System.Net.Sockets;
using System.Text;

// Include the ProtoIP namespace
using ProtoIP;

class ComplexServer : ProtoServer
{
      // Once the user makes a request, we can build the packet from the protoStream
      // and respond accordingly
      public override void OnRequest(int userID)
      {
            // Get the data from the ProtoStream and deserialize the packet
            byte[] data = _protoStreamArrayClients[userID].GetDataAs<byte[]>();
            Packet receivedPacket = Packet.Deserialize(data);

            // Respond to PING packets
            if (receivedPacket._GetType() == Packet.Type.PING)
            {
                  Packet packet = new Packet(Packet.Type.PONG);
                  Send(packet.Serialize(), userID);
            }
      }
}

class Program 
{
      const int PORT = 1234;

      static void Main()
      {
            // Create the server and start it
            ComplexServer server = new ComplexServer(PORT);
            server.Start();
      }
}

exampleComplexClient
using System;
using System.Net.Sockets;
using System.Text;

// Include the ProtoIP namespace
using ProtoIP;

class ComplexClient : ProtoClient 
{
      // We can override the OnConnect() method to describe our Client logic
      // In this example, we are printing out the message we get back from the
      // remote host.
      public override void OnReceive() 
      {
            string data = _protoStream.GetDataAs<string>();
            Console.WriteLine(data);
      }
}

class Program 
{
      static void Main() 
      {
            // Create a new ComplexClient object and connect to the server
            ComplexClient client = new ComplexClient();
            client.Connect("1.1.1.1", 1234);

            // Send data to the server
            client.Send("Hello World!");

            // Receive the response
            // The OnReceive() method will be called
            client.Receive();

            // Disconnect from the server
            client.Disconnect();
      }
}


Packet type from ProtoIP.DLL:


    public Packet()
    {
    }

    public Packet(int type, int id, int dataSize, byte[] data)
    {
        _type = type;
        _id = id;
        _dataSize = dataSize;
        _data = data;
    }

    public Packet(int type, int id, int dataSize, string data)
    {
        _type = type;
        _id = id;
        _dataSize = dataSize;
        _data = Encoding.ASCII.GetBytes(data);
    }

    public Packet(Type type, int id, int dataSize, byte[] data)
    {
        _type = (int)type;
        _id = id;
        _dataSize = dataSize;
        _data = data;
    }

    public Packet(int type)
    {
        _type = type;
        _id = 0;
        _dataSize = 0;
        _data = new byte[0];
    }

    public Packet(Type type)
    {
        _type = (int)type;
        _id = 0;
        _dataSize = 0;
        _data = new byte[0];
    }

    public Packet(string stringData)
    {
        _type = 6;
        _id = 0;
        _dataSize = stringData.Length;
        _data = Encoding.ASCII.GetBytes(stringData);
    }

    public static byte[] Serialize(Packet packet)
    {
        byte[] array = new byte[1024];
        byte[] bytes = BitConverter.GetBytes(packet._type);
        byte[] bytes2 = BitConverter.GetBytes(packet._id);
        byte[] bytes3 = BitConverter.GetBytes(packet._dataSize);
        byte[] data = packet._data;
        Buffer.BlockCopy(bytes, 0, array, 0, bytes.Length);
        Buffer.BlockCopy(bytes2, 0, array, bytes.Length, bytes2.Length);
        Buffer.BlockCopy(bytes3, 0, array, bytes.Length + bytes2.Length, bytes3.Length);
        Buffer.BlockCopy(data, 0, array, bytes.Length + bytes2.Length + bytes3.Length, data.Length);
        if (array.Length < 1024)
        {
            byte[] array2 = new byte[1024 - array.Length];
            Buffer.BlockCopy(array2, 0, array, array.Length, array2.Length);
        }

        return array;
    }


Server ex:

using ProtoIP;

class ComplexServer : ProtoServer
{
      // Once the user makes a request, we can build the packet from the protoStream
      // and respond accordingly
      public override void OnRequest(int userID)
      {
            // Get the data from the ProtoStream and deserialize the packet
            byte[] data = _clients[userID].GetDataAs<byte[]>();
            Packet receivedPacket = Packet.Deserialize(data);

            // Respond to PING packets
            if (receivedPacket._GetType() == (int)Packet.Type.PING)
            {
                  Packet packet = new Packet(Packet.Type.PONG);
                  Send(packet.Serialize(), userID);
            }
      }
}

class Program 
{
      const int PORT = 1234;

      static void Main()
      {
            // Create the server and start it
            ComplexServer server = new ComplexServer(PORT);
            server.Start();
      }
}


client ex:

using ProtoIP;

class ComplexClient : ProtoClient 
{
      public override void OnReceive() 
      {
            string data = _protoStream.GetDataAs<string>();
            Console.WriteLine(data);
      }
}

class Program 
{
      static void Main() 
      {
            // Create a new ComplexClient object
            ComplexClient client = new ComplexClient();

            // Connect to the server
            client.Connect("1.1.1.1", 1234);

            // Send a string to the server
            client.Send("Hello World!");

            // Receive the response
            // The OnReceive() method will be called
            client.Receive();

            // Disconnect from the server
            client.Disconnect();
      }
}
*/

// WS server e client
using ProtoIP;
using Microsoft.Graph.Models;

namespace chatlib.server
{
    // SERVIDOR
    public class ChatServer : ProtoServer
    {
        private UserManager _userManager; // Gerenciador de usuarios
        private int _port; // Porta do servidor

        public ChatServer(int port)
        {
            // Cria o servidor e inicia a escuta na porta especificada
            _userManager = new UserManager();
            _port = port;
        }

        public void Start()
        {
            // Inicia o servidor
            Start(_port);
        }

        public byte[] StringToByteArr(string str)
        {
            // Converte uma string para um array de bytes
            return Encoding.UTF8.GetBytes(str);
        }

        //if (data == null || data.Length < 12) // 12 bytes: 3 inteiros (type, id, dataSize)
        //{
        //    Console.WriteLine("Buffer recebido é nulo ou muito pequeno.");
        //    throw new Exception("Buffer inválido para deserialização do pacote.");
        //}

        //// Opcional: validar se o tamanho do payload está correto
        //int dataSize = BitConverter.ToInt32(data, 8); // offset 8: após type (4) + id (4)
        //if (data.Length < 12 + dataSize)
        //{
        //    Console.WriteLine("Tamanho do buffer não corresponde ao tamanho esperado do payload.");
        //    throw new Exception("Buffer corrompido ou incompleto.");
        //}

        /*

         */

        // Só depois de validar, faça a desserialização
        //Packet receivedPacket = Packet.Deserialize(data);

        // Atualmente possui um erro nesta parte.
        //var packet = AssembleReceivedDataIntoPacket(userID); // Erro aqui, pois o método não existe na classe ProtoServer
        //var packetType = packet._GetType();
        //var packetData = packet.GetDataAs<string>();
        ////var packetJson = JsonSerializer.Deserialize<Packet>(packetData);

        //// Verifica se o pacote recebido é válido
        //if (string.IsNullOrEmpty(packetData))
        //{
        //    // Se o pacote não contiver dados, lança uma exceção
        //    throw new Exception("Recebido pacote inválido");
        //}
        //Console.WriteLine($"Recebido pacote do usuario {userID} com tipo {packetType} e dados: {packetData}");
        //// Verifica se o pacote é do tipo mensagem

        // Substitua o método Deserialize para não tentar acessar a propriedade inexistente "Data" em Packet.
        // Em vez disso, utilize o construtor adequado de Packet ou remova o método se não for necessário.
        // Exemplo de correção removendo o método problemático:

        // Remova ou comente o método abaixo, pois Packet não possui a propriedade "Data":
        /*
        public static Packet Deserialize(byte[] buffer)
        {
            if (buffer == null || buffer.Length < 4)
                throw new ArgumentException("Buffer muito pequeno para conter cabeçalho.");

            int offset = 0;

            // Supondo que os primeiros 4 bytes indiquem o tamanho do payload
            int payloadLength = BitConverter.ToInt32(buffer, offset);
            offset += 4;

            if (payloadLength < 0 || offset + payloadLength > buffer.Length)
                throw new ArgumentException("Tamanho inválido do payload ou buffer incompleto.");

            byte[] payload = new byte[payloadLength];
            Buffer.BlockCopy(buffer, offset, payload, 0, payloadLength);

            // Não existe Packet.Data, então use o construtor adequado ou remova este método
            // return new Packet { Data = payload };
            throw new NotImplementedException("Packet não possui propriedade Data. Use Packet.Deserialize(buffer) da própria biblioteca.");
        }
        */

        // Use sempre Packet.Deserialize(buffer) da biblioteca ProtoIP para desserializar pacotes.

        /*
         Client side:
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

                // Cria o pacote e envia
                Packet packet = new Packet(Packet.Type.MESSAGE, json);
                this.Send(packet.Serialize());
            }
         */
        public override void OnRequest(int userID)
        {
            try
            {
                var packet = AssembleReceivedDataIntoPacket(userID);

                if (packet == null)
                {
                    Console.WriteLine($"[WARN] Pacote nulo de {userID}.");
                    return;
                }

                var packetType = packet._GetType();
                var packetData = packet.GetDataAs<string>();

                if (string.IsNullOrEmpty(packetData))
                {
                    throw new Exception("Recebido pacote inválido (dados vazios).");
                }

                Console.WriteLine($"[SERVER] Pacote recebido de {userID}: {packetType}, Conteúdo: {packetData}");

                if (packetType == (int)Packet.Type.PING)
                {
                    var pong = new Packet(Packet.Type.PONG);
                    Send(Packet.Serialize(pong), userID);
                }

                // Outras lógicas de tratamento aqui...
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Erro ao processar requisição do usuário {userID}: {ex.Message}");
            }
        }

        public Packet AssembleReceivedDataIntoPacket(int userID)
        {
            try
            {
                byte[] data = _clients[userID].GetDataAs<byte[]>();

                if (data == null || data.Length < 4) // Assuma 4 bytes como tamanho mínimo, ajuste conforme estrutura do Packet
                {
                    Console.WriteLine($"[WARN] Dados inválidos de {userID}. Tamanho: {data?.Length}");
                    return null;
                }

                return Packet.Deserialize(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Erro ao desserializar pacote do usuário {userID}: {ex.Message}");
                return null;
            }
        }
    }
}
