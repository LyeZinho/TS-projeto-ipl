using System;
using System.Linq;
using System.Text;
using chatlib.objects;
using System.Text.Json;

/*
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

*/

// WS server e client
using ProtoIP;

namespace chatlib.server
{
    // SERVIDOR
    public class ChatServer : ProtoServer
    {
        private UserManager _userManager; // Gerenciador de usuarios

        public ChatServer(string ip, int port, bool ssl = false)
        {
            // Cria o servidor e inicia a escuta na porta especificada
            _userManager = new UserManager();
        }

        public void Start()
        {
            // Inicia o servidor
            Start(8080);
        }

        public byte[] StringToByteArr(string str)
        {
            // Converte uma string para um array de bytes
            return Encoding.UTF8.GetBytes(str);
        }

        public override void OnRequest(int userID)
        {
            // Lida com as requisições recebidas
            /*
            Formato do pacote:
            {
                "messagetype": "message",
                "sender": "username",
                "target": "targetUsername",
                "publicKey": "chavePublica",
                "message": "mensagem"
                "data": { // dados adicionais, se necessário
                    "key1": "value1",
                    "key2": "value2"
                },
                "timestamp": "2023-10-01T12:00:00Z"
            }
            */

            // Deserializa o pacote recebido
            var packet = AssembleReceivedDataIntoPacket(userID);
            var packetType = packet._GetType();
            var packetData = packet.GetDataAs<string>();
            var packetJson = JsonSerializer.Deserialize<Packet>(packetData);

            var packetDict = JsonSerializer.Deserialize<Dictionary<string, object>>(packetData);
            var packetMessageTypeStr = packetDict["messagetype"].ToString() ?? string.Empty;
            var packetSender = packetDict["sender"].ToString() ?? string.Empty;
            var packetTarget = packetDict["target"].ToString() ?? string.Empty;
            var packetPublicKey = packetDict["publicKey"].ToString() ?? string.Empty;
            var packetMessage = packetDict["message"].ToString() ?? string.Empty;
            var packetDataJson = packetDict["data"].ToString() ?? string.Empty;
            var packetTimestamp = packetDict["timestamp"].ToString() ?? string.Empty;
            var packetDataObj = JsonSerializer.Deserialize<Dictionary<string, object>>(packetDataJson);
            
            if (
                string.IsNullOrEmpty(packetMessageTypeStr)
                || string.IsNullOrEmpty(packetSender)
                || string.IsNullOrEmpty(packetTarget)
                || string.IsNullOrEmpty(packetPublicKey)
                || string.IsNullOrEmpty(packetMessage)
                || string.IsNullOrEmpty(packetTimestamp)
            )
            {
                throw new Exception("Recebido pacote inválido");
            }

            // Verifica o tipo do pacote e executa a ação correspondente
            switch (packetMessageTypeStr)
            {
                case "message":
                    // Envia a mensagem para o usuário de destino
                    var targetUser = _userManager.GetUser(packetTarget);
                    if (targetUser != null)
                    {
                        Send(StringToByteArr(packetData), targetUser.Id);
                    }
                    break;
                case "friend-add":
                    // Busca o usuário que deseja adicionar comoo amigo e retorna a chave pública do mesmo
                    var friendUsername = packetDict["friendUsername"].ToString() ?? string.Empty;
                    var friendPublicKey = packetDict["friendPublicKey"].ToString() ?? string.Empty;
                    var friendUser = _userManager.GetUser(friendUsername);

                    if (friendUser != null)
                    {
                        var Payload = new
                        {
                            friendUsername = friendUser.Username,
                            friendPublicKey = friendUser.PublicKey,
                            message = "Adicionado como amigo com sucesso",
                            timestamp = DateTime.UtcNow.ToString("o"),
                        };

                        Send(StringToByteArr(JsonSerializer.Serialize(Payload)), userID);
                    }
                    else
                    {
                        throw new Exception("Usuário não encontrado");
                    }

                    break;
                default:
                    throw new Exception("Tipo de pacote desconhecido");
            }             
        }
    }


    
}
