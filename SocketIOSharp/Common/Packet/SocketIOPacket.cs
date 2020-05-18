using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocketIOSharp.Common.Packet
{
    public partial class SocketIOPacket
    {
        public SocketIOPacketType Type { get; private set; }

        public Queue<SocketIOPacket> Attachments { get; private set; }
        public string Namespace { get; private set; }
        public int ID { get; private set; }

        public bool IsJson { get; private set; }
        public bool IsBinary { get; private set; }

        public JToken JsonData { get; set; }
        public byte[] RawData { get; private set; }

        private SocketIOPacket()
        {
            Type = SocketIOPacketType.UNKNOWN;

            Attachments = new Queue<SocketIOPacket>();
            Namespace = "/";
            ID = -1;

            IsJson = false;
            IsBinary = false;

            JsonData = null;
            RawData = null;
        }

        private SocketIOPacket(SocketIOPacket Packet)
        {
            Type = Packet.Type;

            if (Packet.Attachments != null)
            {
                Attachments = new Queue<SocketIOPacket>(Packet.Attachments);
            }
            else
            {
                Attachments = null;
            }

            Namespace = Packet.Namespace;
            ID = Packet.ID;

            IsJson = Packet.IsJson;
            IsBinary = Packet.IsBinary;

            if (Packet.JsonData != null)
            {
                JsonData = Packet.JsonData.DeepClone();
            }
            else
            {
                JsonData = null;
            }

            if (Packet.RawData != null)
            {
                RawData = new List<byte>(Packet.RawData).ToArray();
            }
            else
            {
                RawData = null;
            }
        }

        public SocketIOPacket DeepClone()
        {
            return new SocketIOPacket(this);
        }

        public override string ToString()
        {
            StringBuilder Builder = new StringBuilder(string.Format
            (
                "Packet: SocketPacketType={0}, ", 
                Type
            ));

            if (Attachments != null && Attachments.Count > 0)
            {
                Builder.Append("Attachments=[");

                foreach (SocketIOPacket Attachment in Attachments)
                {
                    Builder.Append(Attachment.ToString());
                }

                Builder.Append("], ");
            }

            Builder.Append(string.Format("Namespace={0}, ID={1}", Namespace, ID));

            if (JsonData != null)
            {
                Builder.Append(string.Format(", JsonData={0}", JsonData));
            }

            if (RawData != null)
            {
                Builder.Append(string.Format(", BinaryData={0}", BitConverter.ToString(RawData)));
            }

            return Builder.ToString();
        }

        internal object Encode()
        {
            try
            {
                if (IsJson)
                {
                    StringBuilder Builder = new StringBuilder();

                    if (Type != SocketIOPacketType.UNKNOWN)
                    {
                        Builder.Append((int)Type);
                    }

                    if (Type == SocketIOPacketType.BINARY_EVENT || Type == SocketIOPacketType.BINARY_ACK)
                    {
                        Builder.Append(Attachments.Count);
                        Builder.Append('-');
                    }

                    if (!string.IsNullOrEmpty(Namespace) && !Namespace.Equals("/"))
                    {
                        Builder.Append(Namespace);
                        Builder.Append(',');
                    }

                    if (ID > -1)
                    {
                        Builder.Append(ID);
                    }

                    return Builder.Append(JsonData?.ToString(Formatting.None) ?? string.Empty).ToString();
                }
                else
                {
                    if (Type != SocketIOPacketType.UNKNOWN)
                    {
                        List<byte> Buffer = new List<byte>() { (byte)Type };
                        Buffer.AddRange(RawData);

                        return Buffer.ToArray();
                    }
                    else
                    {
                        return RawData;
                    }
                }
            }
            catch (Exception Exception)
            {
                throw new SocketIOException("Packet encoding failed. " + this, Exception);
            }
        }
    }
}