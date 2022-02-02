using EngineIOSharp.Common.Packet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocketIOSharp.Common.Packet
{
    partial class SocketIOPacket
    {
        internal static SocketIOPacket Decode(EngineIOPacket EngineIOPacket)
        {
            if ((EngineIOPacket?.Type ?? EngineIOPacketType.UNKNOWN) == EngineIOPacketType.MESSAGE)
            {
                try
                {
                    if (EngineIOPacket.IsText)
                    {
                        return Decode(EngineIOPacket.Data);
                    } 
                    else
                    {
                        return Decode(EngineIOPacket.RawData);
                    }
                }
                catch (Exception Exception)
                {
                    throw new SocketIOException("Packet decoding failed. " + EngineIOPacket, Exception);
                }
            }
            else
            {
                throw new SocketIOException("Type of Engine.IO packet is not message.");
            }
        }

        internal static SocketIOPacket Decode(string Data)
        {
            SocketIOPacket Packet = new SocketIOPacket();
            int Offset = 0;

            Packet.Type = (SocketIOPacketType)(Data[Offset] - '0');

            if (Data.Length > 1)
            {
                if (Packet.Type == SocketIOPacketType.BINARY_EVENT || Packet.Type == SocketIOPacketType.BINARY_ACK)
                {
                    StringBuilder Builder = new StringBuilder();

                    while (Offset < Data.Length - 1)
                    {
                        char c = Data[++Offset];

                        if (char.IsNumber(c))
                        {
                            Builder.Append(c);
                        }
                        else
                        {
                            break;
                        }
                    }

                    Packet.Attachments = new Queue<SocketIOPacket>(new SocketIOPacket[int.Parse(Builder.ToString())]);
                }

                if ('/' == Data[Offset + 1])
                {
                    StringBuilder Builder = new StringBuilder();

                    while (Offset < Data.Length - 1 && Data[++Offset] != ',')
                    {
                        Builder.Append(Data[Offset]);
                    }

                    Packet.Namespace = Builder.ToString();
                }
                else
                {
                    Packet.Namespace = "/";
                }

                if (Offset < Data.Length - 1)
                {
                    char Next = Data[Offset + 1];

                    if (!char.IsWhiteSpace(Next) && char.IsNumber(Next))
                    {
                        StringBuilder Builder = new StringBuilder();

                        while (Offset < Data.Length - 1)
                        {
                            char c = Data[++Offset];

                            if (char.IsNumber(c))
                            {
                                Builder.Append(c);
                            }
                            else
                            {
                                --Offset;
                                break;
                            }
                        }

                        Packet.ID = int.Parse(Builder.ToString());
                    }
                }

                if (++Offset < Data.Length - 1)
                {
                    Packet.JsonData = (JToken)JsonConvert.DeserializeObject(Data.Substring(Offset));
                    Packet.IsJson = true;
                }
            }

            return Packet;
        }

        internal static SocketIOPacket Decode(byte[] RawData)
        {
            return new SocketIOPacket()
            {
                RawData = RawData,
                IsJson = false,
                IsBinary = true
            };
        }
    }
}
