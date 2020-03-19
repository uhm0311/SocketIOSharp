using SocketIOSharp.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketIOSharp.Packet
{
    partial class SocketIOPacket
    {
        public static object Encode(SocketIOPacket Packet)
        {
            try
            {
                if (Packet.IsBinary)
                {
                    List<byte> RawData = new List<byte> { (byte)Packet.EnginePacketType };
                    RawData.AddRange(Packet.BinaryData);

                    return RawData.ToArray();
                }
                else
                {
                    StringBuilder Builder = new StringBuilder();

                    if (Packet.EnginePacketType.Equals(EngineIOPacketType.UNKNOWN))
                    {
                        return Builder.ToString();
                    }

                    Builder.Append((int)Packet.EnginePacketType);

                    if (!Packet.EnginePacketType.Equals(EngineIOPacketType.MESSAGE))
                    {
                        return Builder.ToString();
                    }

                    if (!Packet.SocketPacketType.Equals(SocketIOPacketType.UNKNOWN))
                    {
                        Builder.Append((int)Packet.SocketPacketType);
                    }

                    if (Packet.SocketPacketType == SocketIOPacketType.BINARY_EVENT || Packet.SocketPacketType == SocketIOPacketType.BINARY_ACK)
                    {
                        Builder.Append(Packet.Attachments.Count);
                        Builder.Append('-');
                    }

                    if (!string.IsNullOrEmpty(Packet.Namespace) && !Packet.Namespace.Equals("/"))
                    {
                        Builder.Append(Packet.Namespace);
                        Builder.Append(',');
                    }

                    if (Packet.ID > -1)
                    {
                        Builder.Append(Packet.ID);
                    }

                    if (Packet.IsJson)
                    {
                        if (Packet != null)
                        {
                            Builder.Append(Packet.JsonData.ToString());
                        }
                        else
                        {
                            Builder.Append("[null]");
                        }
                    }

                    return Builder.ToString();
                }
            }
            catch (Exception e)
            {
                throw new SocketIOClientException("Packet encoding failed. " + Packet, e);
            }
        }
    }
}
