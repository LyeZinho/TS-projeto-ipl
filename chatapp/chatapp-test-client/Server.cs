using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoIP;

// Server
class PingPongServer : ProtoServer
{
    public override void OnRequest(int userID)
    {
        /*
         * Sent from the client:
         Packet pingPacket = new Packet(Packet.Type.BYTES);
        pingPacket.SetPayload(
            Encoding.UTF8.GetBytes("PING: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
        );
        client.Send(
            Packet.Serialize(pingPacket) // Serialize the packet to byte array
            );
         */
        Packet receivedPacket = AssembleReceivedDataIntoPacket(userID);

        // Recive the data:

        if (receivedPacket != null)
        {
            Console.WriteLine("Received packet from user " + userID + ": " + receivedPacket.ToString());
        }
        else
        {
            Console.WriteLine("Received null packet from user " + userID);
        }
    }
}