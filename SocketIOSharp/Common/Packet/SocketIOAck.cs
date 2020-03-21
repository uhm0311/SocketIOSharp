using Newtonsoft.Json.Linq;
using SocketIOSharp.Common.Action;
using System;

namespace SocketIOSharp.Common.Packet
{
    internal class SocketIOAck
    {
        public DateTime RequestedTime { get; private set; }

        public int PacketID { get; private set; }
        public SocketIOAction.Event Callback { get; private set; }

        internal SocketIOAck(int PacketID, SocketIOAction.Event Callback = null)
        {
            RequestedTime = DateTime.UtcNow;

            this.PacketID = PacketID;
            this.Callback = Callback;
        }

        public void Invoke(params JToken[] Data)
        {
            Callback(Data);
        }

        public override string ToString()
        {
            return string.Format
            (
                "[Ack: PacketID={0}, RequestedTime={1}, Action={2}]",
                PacketID,
                RequestedTime,
                Callback
            );
        }
    }
}
