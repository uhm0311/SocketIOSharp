using EngineIOSharp.Common.Enum;
using EngineIOSharp.Server.Client;
using SocketIOSharp.Common.Abstract.Connection;

namespace SocketIOSharp.Server.Client
{
    public partial class SocketIOSocket : SocketIOConnection
    {
        private readonly EngineIOSocket Socket;

        public SocketIOServer Server { get; private set; }

        public override EngineIOReadyState ReadyState => Socket.ReadyState;

        internal SocketIOSocket(EngineIOSocket Socket, SocketIOServer Server)
        {
            UseAckTimeout = Server.Option.UseAckTimeout;
            AckManager.SetTimeout(Server.Option.PingTimeout);

            Socket.OnMessage(OnPacket);
            Socket.OnClose(OnDisconnect);

            this.Server = Server;
            this.Socket = Socket;
        }

        public override void Close()
        {
            Socket.Close();
        }
    }
}
