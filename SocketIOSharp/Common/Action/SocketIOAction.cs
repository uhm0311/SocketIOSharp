using Newtonsoft.Json.Linq;

namespace SocketIOSharp.Common.Action
{
    public static class SocketIOAction
    {
        public delegate void Event(JToken[] Data);
        public delegate void Ack(JToken[] Data, Event Callback);
    }
}
