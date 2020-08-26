using Newtonsoft.Json.Linq;
using System;

namespace SocketIOSharp.Common.Packet
{
    public class SocketIOAck
    {
        public DateTime RequestedTime { get; private set; }

        public int PacketID { get; private set; }
        public Action<JToken[]> Callback { get; private set; }

        internal SocketIOAck(int PacketID, Action<JToken[]> Callback = null)
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
