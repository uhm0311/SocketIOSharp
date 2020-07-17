using EngineIOSharp.Common.Enum;
using EngineIOSharp.Server.Client;
using SocketIOSharp.Common.Abstract.Connection;

namespace SocketIOSharp.Server.Client
{
    public partial class SocketIOSocket : SocketIOConnection<SocketIOSocket>
    {
        private readonly EngineIOSocket Socket;

        public SocketIOServer Server { get; private set; }

        public override EngineIOReadyState ReadyState => Socket.ReadyState;

        internal SocketIOSocket(EngineIOSocket Socket, SocketIOServer Server)
        {
            AckManager.SetTimeout(Server.Option.PingTimeout);

            Socket.OnMessage(OnPacket);
            Socket.OnClose((message, description) => OnDisconnect(description));

            this.Server = Server;
            this.Socket = Socket;
        }

        public override SocketIOSocket Close()
        {
            Socket.Close();

            return this;
        }
    }
}
