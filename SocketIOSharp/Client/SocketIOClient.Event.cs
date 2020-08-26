namespace SocketIOSharp.Client
{
    partial class SocketIOClient
    {
        private static class Event
        {
            public static readonly string CONNECT_ERROR = "connect_error";

            public static readonly string RECONNECT = "reconnect";
            public static readonly string RECONNECT_ATTEMPT = "reconnect_attempt";
            public static readonly string RECONNECTING = "reconnecting";

            public static readonly string RECONNECT_ERROR = "reconnect_error";
            public static readonly string RECONNECT_FAILED = "reconnect_failed";

            public static readonly string PING = "ping";
            public static readonly string PONG = "pong";
        }
    }
}
