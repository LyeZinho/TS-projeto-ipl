using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoIP;

// Client
class PingPongClient : ProtoClient
{
    public override void OnReceive()
    {
        Packet receivedPacket = AssembleReceivedDataIntoPacket();

        if (receivedPacket._GetType() == (int)Packet.Type.BYTES)
        {
            Console.WriteLine("CLIENT: Received PONG packet!");
        }
    }
}