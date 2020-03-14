using Newtonsoft.Json.Linq;
using System;

namespace SocketIOSharp.Packet.Ack
{
    internal class SocketIOAck
    {
        public DateTime RequestedTime { get; private set; }

        public int PacketID { get; private set; }
        public Action<JToken[]> Action { get; private set; }

        internal SocketIOAck(int PacketID, Action<JToken[]> Action = null)
        {
            this.PacketID = PacketID;
            this.RequestedTime = DateTime.UtcNow;

            this.Action = Action;
        }

        public void Invoke(params JToken[] Data)
        {
            this.Action(Data);
        }

        public override string ToString()
        {
            return string.Format("[Ack: PacketID={0}, RequestedTime={1}, Action={2}]", PacketID, RequestedTime, Action);
        }
    }
}
