using EngineIOSharp.Server;
using SocketIOSharp.Common.Abstract;
using SocketIOSharp.Server.Client;
using System.Collections.Generic;

namespace SocketIOSharp.Server
{
    public partial class SocketIOServer : SocketIO
    {
        private readonly EngineIOServer Server;

        private readonly List<SocketIOSocket> _Clients = new List<SocketIOSocket>();
        private readonly object ClientMutex = new object();

        public List<SocketIOSocket> Clients { get { return new List<SocketIOSocket>(_Clients); } }
        public int ClientsCounts { get { return _Clients.Count; } }

        public SocketIOServerOption Option { get; private set; }

        public SocketIOServer(SocketIOServerOption Option)
        {
            UseAckTimeout = Option.UseAckTimeout;

            Server = new EngineIOServer(this.Option = Option);
            Server.OnConnection(OnConnection);
        }

        public void Start()
        {
            Server.Start();
        }

        public void Stop()
        {
            Server.Stop();
        }

        public override void Dispose()
        {
            Stop();
        }
    }
}
