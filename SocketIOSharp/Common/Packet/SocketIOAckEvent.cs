using Newtonsoft.Json.Linq;
using System;

namespace SocketIOSharp.Common.Packet
{
    public class SocketIOAckEvent
    {
        public JToken[] Data { get; private set; }
        public Action<JToken[]> Callback { get; private set; }

        internal SocketIOAckEvent(JToken[] Data, Action<JToken[]> Callback)
        {
            this.Data = Data;
            this.Callback = Callback;
        }
    }
}
