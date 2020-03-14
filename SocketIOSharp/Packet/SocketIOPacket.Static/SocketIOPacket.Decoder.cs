using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SocketIOSharp.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocketIOSharp.Packet
{
    partial class SocketIOPacket
    {
        public static SocketIOPacket Decode(byte[] RawData)
        {
            try
            {
                SocketIOPacket Packet = new SocketIOPacket();
                Queue<byte> BufferQueue = new Queue<byte>(RawData);

                return Decode((EngineIOPacketType)BufferQueue.Dequeue(), BufferQueue.ToArray());
            }
            catch (Exception ex)
            {
                StringBuilder Builder = new StringBuilder();
                if (RawData != null)
                    Builder.Append(BitConverter.ToString(RawData));

                throw new SocketIOClientException("Packet decoding failed. " + Builder, ex);
            }
        }

        public static SocketIOPacket Decode(EngineIOPacketType EnginePacketType, byte[] BinaryData)
        {
            SocketIOPacket Packet = new SocketIOPacket();

            Packet.EnginePacketType = EnginePacketType;
            Packet.BinaryData = new List<byte>(BinaryData).ToArray();
            Packet.IsBinary = true;

            return Packet;
        }

        public static SocketIOPacket Decode(string PacketString)
        {
            try
            {
                SocketIOPacket Packet = new SocketIOPacket();
                int Offset = 0;

                if ((Packet.EnginePacketType = (EngineIOPacketType)(PacketString[Offset] - '0')) == EngineIOPacketType.MESSAGE)
                    Packet.SocketPacketType = (SocketIOPacketType)(PacketString[++Offset] - '0');
                if (PacketString.Length <= 2)
                    return Packet;

                if (Packet.SocketPacketType == SocketIOPacketType.BINARY_EVENT || Packet.SocketPacketType == SocketIOPacketType.BINARY_ACK)
                {
                    StringBuilder Builder = new StringBuilder();
                    while (Offset < PacketString.Length - 1)
                    {
                        char c = PacketString[++Offset];
                        if (char.IsNumber(c))
                            Builder.Append(c);
                        else break;
                    }
                    Packet.Attachments = new Queue<SocketIOPacket>(new SocketIOPacket[int.Parse(Builder.ToString())]);
                }

                if ('/' == PacketString[Offset + 1])
                {
                    StringBuilder Builder = new StringBuilder();
                    while (Offset < PacketString.Length - 1 && PacketString[++Offset] != ',')
                        Builder.Append(PacketString[Offset]);
                    Packet.Namespace = Builder.ToString();
                }
                else Packet.Namespace = "/";

                char Next = PacketString[Offset + 1];
                if (!char.IsWhiteSpace(Next) && char.IsNumber(Next))
                {
                    StringBuilder Builder = new StringBuilder();
                    while (Offset < PacketString.Length - 1)
                    {
                        char c = PacketString[++Offset];
                        if (char.IsNumber(c))
                            Builder.Append(c);
                        else
                        {
                            --Offset;
                            break;
                        }
                    }
                    Packet.ID = int.Parse(Builder.ToString());
                }

                if (++Offset < PacketString.Length - 1)
                {
                    Packet.JsonData = (JToken)JsonConvert.DeserializeObject(PacketString.Substring(Offset));
                    Packet.IsJson = true;
                }

                return Packet;
            }
            catch (Exception ex)
            {
                throw new SocketIOClientException("Packet decoding failed. " + PacketString, ex);
            }
        }
    }
}
