using EngineIOSharp.Client;
using EngineIOSharp.Common.Enum;
using SocketIOSharp.Common.Abstract.Connection;

namespace SocketIOSharp.Client
{
    public partial class SocketIOClient : SocketIOConnection
    {
        private readonly EngineIOClient Client;

        public SocketIOClientOption Option { get; private set; }
        public bool AutoReconnect { get; set; }

        public override EngineIOReadyState ReadyState => Client.ReadyState;

        public SocketIOClient(SocketIOClientOption Option)
        {
            AutoReconnect = Option.AutoReconnect;
            UseAckTimeout = Option.UseAckTimeout;

            Client = new EngineIOClient(this.Option = Option);
            Client.OnOpen(() =>
            {
                AckManager.SetTimeout(Client.Handshake.PingTimeout);
                Closed = false;
            });

            Client.OnMessage(OnPacket);
            Client.OnClose(() =>
            {
                OnDisconnect();

                if (AutoReconnect)
                {
                    Connect();
                }
            });
        }

        public void Connect()
        {
            Client.Connect();
        }

        public override void Close()
        {
            AutoReconnect = false;

            Client.Close();
            AckManager.Dispose();
            Reconstructor.Dispose();
        }
    }
}