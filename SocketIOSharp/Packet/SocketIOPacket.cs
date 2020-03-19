using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocketIOSharp.Packet
{
    internal partial class SocketIOPacket
    {
        public EngineIOPacketType EnginePacketType { get; private set; }
        public SocketIOPacketType SocketPacketType { get; private set; }

        public Queue<SocketIOPacket> Attachments { get; private set; }
        public string Namespace { get; private set; }
        public int ID { get; private set; }

        public bool IsJson { get; private set; }
        public bool IsBinary { get; private set; }

        public JToken JsonData { get; set; }
        public byte[] BinaryData { get; private set; }

        private SocketIOPacket()
        {
            EnginePacketType = EngineIOPacketType.UNKNOWN;
            SocketPacketType = SocketIOPacketType.UNKNOWN;

            Attachments = new Queue<SocketIOPacket>();
            Namespace = "/";
            ID = -1;

            IsJson = false;
            IsBinary = false;

            JsonData = null;
            BinaryData = null;
        }

        private SocketIOPacket(SocketIOPacket Packet)
        {
            EnginePacketType = Packet.EnginePacketType;
            SocketPacketType = Packet.SocketPacketType;

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

            if (Packet.BinaryData != null)
            {
                BinaryData = new List<byte>(Packet.BinaryData).ToArray();
            }
            else
            {
                BinaryData = null;
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
                "Packet: EnginePacketType={0}, SocketPacketType={1}, ", 
                EnginePacketType, 
                SocketPacketType
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

            if (BinaryData != null)
            {
                Builder.Append(string.Format(", BinaryData={0}", BitConverter.ToString(BinaryData)));
            }

            return Builder.ToString();
        }
    }
}