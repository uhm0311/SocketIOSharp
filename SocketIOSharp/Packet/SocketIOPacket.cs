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
            this.EnginePacketType = EngineIOPacketType.UNKNOWN;
            this.SocketPacketType = SocketIOPacketType.UNKNOWN;

            this.Attachments = new Queue<SocketIOPacket>();
            this.Namespace = "/";
            this.ID = -1;

            this.IsJson = false;
            this.IsBinary = false;

            this.JsonData = null;
            this.BinaryData = null;
        }

        private SocketIOPacket(SocketIOPacket Packet)
        {
            this.EnginePacketType = Packet.EnginePacketType;
            this.SocketPacketType = Packet.SocketPacketType;

            if (Packet.Attachments != null)
                this.Attachments = new Queue<SocketIOPacket>(Packet.Attachments);
            else this.Attachments = null;

            this.Namespace = Packet.Namespace;
            this.ID = Packet.ID;

            this.IsJson = Packet.IsJson;
            this.IsBinary = Packet.IsBinary;

            if (Packet.JsonData != null)
                this.JsonData = Packet.JsonData.DeepClone();
            else this.JsonData = null;

            if (Packet.BinaryData != null)
                this.BinaryData = new List<byte>(Packet.BinaryData).ToArray();
            else this.BinaryData = null;
        }

        public SocketIOPacket DeepClone()
        {
            return new SocketIOPacket(this);
        }

        public override string ToString()
        {
            StringBuilder String = new StringBuilder(string.Format("Packet: EnginePacketType={0}, SocketPacketType={1}, ", EnginePacketType, SocketPacketType));
            if (Attachments != null && Attachments.Count > 0)
            {
                String.Append("Attachments=[");
                foreach (SocketIOPacket Attachment in Attachments)
                    String.Append(Attachment.ToString());

                String.Append("], ");
            }
            String.Append(string.Format("Namespace={0}, ID={1}", Namespace, ID));

            if (JsonData != null)
                String.Append(string.Format(", JsonData={0}", JsonData));

            if (BinaryData != null)
                String.Append(string.Format(", BinaryData={0}", BitConverter.ToString(BinaryData)));

            return String.ToString();
        }
    }
}