using Newtonsoft.Json.Linq;
using SocketIOSharp.Common.Packet.Binary.Constructors;

namespace SocketIOSharp.Common.Packet
{
    partial class SocketIOPacket
    {
        internal static SocketIOPacket CreateConnectionPacket(string ns = "/")
        {
            return new SocketIOPacket()
            {
                Type = SocketIOPacketType.CONNECT,
                IsJson = true,
                Namespace = ns
            };
        }

        internal static SocketIOPacket CreateEventPacket(JArray JsonArray, SocketIOAck Ack, string ns)
        {
            SocketIOPacket Packet = CreateMessagePacket(JsonArray, Ack, ns);

            if (Packet.IsJson)
            {
                Packet.Type = SocketIOPacketType.EVENT;
            }
            else
            {
                Packet.Type = SocketIOPacketType.BINARY_EVENT;
            }

            Packet.IsJson = true;
            return Packet;
        }

        internal static SocketIOPacket CreateAckPacket(JArray JsonArray, int PacketID, string ns = "/")
        {
            if (PacketID >= 0)
            {
                SocketIOPacket Packet = CreateMessagePacket(JsonArray, PacketID, ns);

                if (Packet.IsJson)
                {
                    Packet.Type = SocketIOPacketType.ACK;
                }
                else
                {
                    Packet.Type = SocketIOPacketType.BINARY_ACK;
                }

                Packet.IsJson = true;
                return Packet;
            }
            else
            {
                return null;
            }
        }

        private static SocketIOPacket CreateMessagePacket(JArray JsonArray, SocketIOAck Ack, string ns)
        {
            return CreateMessagePacket(JsonArray, Ack == null ? -1 : Ack.PacketID, ns);
        }

        private static SocketIOPacket CreateMessagePacket(JArray JsonArray, int PacketID, string ns)
        {
            SocketIOPacket Packet = new SocketIOPacket
            {
                JsonData = JsonArray,
                IsJson = true,
                Namespace = ns
            };

            if (PacketID >= 0)
            {
                Packet.ID = PacketID;
            }

            using (Deconstructor Deconstructor = new Deconstructor())
            {
                Deconstructor.SetPacket(Packet);
                Packet = Deconstructor.Deconstruct();

                if (Packet.Attachments.Count > 0)
                {
                    Packet.IsJson = false;
                }
            }

            return Packet;
        }
    }
}