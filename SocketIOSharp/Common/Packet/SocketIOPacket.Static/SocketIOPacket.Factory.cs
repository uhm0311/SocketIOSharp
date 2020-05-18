using Newtonsoft.Json.Linq;
using SocketIOSharp.Common.Packet.Binary.Constructors;

namespace SocketIOSharp.Common.Packet
{
    partial class SocketIOPacket
    {
        internal static SocketIOPacket CreateConnectionPacket()
        {
            return new SocketIOPacket()
            {
                Type = SocketIOPacketType.CONNECT,
                IsJson = true
            };
        }

        internal static SocketIOPacket CreateEventPacket(JArray JsonArray, SocketIOAck Ack)
        {
            SocketIOPacket Packet = CreateMessagePacket(JsonArray, Ack);

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

        internal static SocketIOPacket CreateAckPacket(JArray JsonArray, int PacketID)
        {
            if (PacketID >= 0)
            {
                SocketIOPacket Packet = CreateMessagePacket(JsonArray, PacketID);

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

        private static SocketIOPacket CreateMessagePacket(JArray JsonArray, SocketIOAck Ack)
        {
            return CreateMessagePacket(JsonArray, Ack == null ? -1 : Ack.PacketID);
        }

        private static SocketIOPacket CreateMessagePacket(JArray JsonArray, int PacketID)
        {
            SocketIOPacket Packet = new SocketIOPacket
            {
                JsonData = JsonArray,
                IsJson = true
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
