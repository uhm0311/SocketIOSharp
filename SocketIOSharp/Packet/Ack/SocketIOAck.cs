using Newtonsoft.Json.Linq;
using System;
using static SocketIOSharp.Client.SocketIOClient;

namespace SocketIOSharp.Packet.Ack
{
    internal class SocketIOAck
    {
        public DateTime RequestedTime { get; private set; }

        public int PacketID { get; private set; }
        public SocketIOEventAction Action { get; private set; }

        internal SocketIOAck(int PacketID, SocketIOEventAction Action = null)
        {
            RequestedTime = DateTime.UtcNow;

            this.PacketID = PacketID;
            this.Action = Action;
        }

        public void Invoke(params JToken[] Data)
        {
            Action(Data);
        }

        public override string ToString()
        {
            return string.Format
            (
                "[Ack: PacketID={0}, RequestedTime={1}, Action={2}]", 
                PacketID, 
                RequestedTime, 
                Action
            );
        }
    }
}
