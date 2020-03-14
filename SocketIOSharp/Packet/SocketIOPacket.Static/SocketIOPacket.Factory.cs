using Newtonsoft.Json.Linq;
using SocketIOSharp.Packet.Ack;
using SocketIOSharp.Packet.Binary.Constructors;

namespace SocketIOSharp.Packet
{
    partial class SocketIOPacket
    {
        internal class Factory
        {
            public static SocketIOPacket CreatePingPacket()
            {
                return new SocketIOPacket() { EnginePacketType = EngineIOPacketType.PING };
            }

            public static SocketIOPacket CreatePongPacket()
            {
                return new SocketIOPacket() { EnginePacketType = EngineIOPacketType.PONG };
            }

            public static SocketIOPacket CreateEventPacket(JArray JsonArray, SocketIOAck Ack, bool JsonOnly)
            {
                SocketIOPacket Packet = CreateMessagePacket(JsonArray, Ack, JsonOnly);

                if (Packet.IsJson)
                    Packet.SocketPacketType = SocketIOPacketType.EVENT;
                else Packet.SocketPacketType = SocketIOPacketType.BINARY_EVENT;

                Packet.IsJson = true;
                return Packet;
            }

            public static SocketIOPacket CreateAckPacket(JArray JsonArray, int PacketID)
            {
                if (PacketID >= 0)
                {
                    SocketIOPacket Packet = CreateMessagePacket(JsonArray, PacketID, false);

                    if (Packet.IsJson)
                        Packet.SocketPacketType = SocketIOPacketType.ACK;
                    else Packet.SocketPacketType = SocketIOPacketType.BINARY_ACK;

                    Packet.IsJson = true;
                    return Packet;
                }
                else return null;
            }

            private static SocketIOPacket CreateMessagePacket(JArray JsonArray, SocketIOAck Ack, bool JsonOnly)
            {
                return CreateMessagePacket(JsonArray, Ack == null ? -1 : Ack.PacketID, JsonOnly);
            }

            private static SocketIOPacket CreateMessagePacket(JArray JsonArray, int PacketID, bool JsonOnly)
            {
                SocketIOPacket Packet = new SocketIOPacket() { EnginePacketType = EngineIOPacketType.MESSAGE };
                Packet.JsonData = JsonArray;
                Packet.IsJson = true;

                if (PacketID >= 0)
                    Packet.ID = PacketID;

                if (!JsonOnly)
                {
                    using (Deconstructor Deconstructor = new Deconstructor())
                    {
                        Deconstructor.SetPacket(Packet);
                        Packet = Deconstructor.Deconstruct();

                        if (Packet.Attachments.Count > 0)
                            Packet.IsJson = false;
                    }
                }

                return Packet;
            }
        }
    }
}
