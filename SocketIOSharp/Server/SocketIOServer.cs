using EngineIOSharp.Server;
using EngineIOSharp.Server.Client;
using SimpleThreadMonitor;
using SocketIOSharp.Common;
using SocketIOSharp.Common.Abstract;
using SocketIOSharp.Common.Packet;
using SocketIOSharp.Server.Client;
using System.Collections.Generic;

namespace SocketIOSharp.Server
{
    public partial class SocketIOServer : SocketIO<SocketIOServer>
    {
        private readonly EngineIOServer Server;

        private readonly List<SocketIOSocket> _Clients = new List<SocketIOSocket>();
        private readonly object ClientMutex = new object();

        public List<SocketIOSocket> Clients { get { return new List<SocketIOSocket>(_Clients); } }
        public int ClientsCounts { get { return _Clients.Count; } }

        public SocketIOServerOption Option { get; private set; }

        public SocketIOServer(SocketIOServerOption Option)
        {
            Server = new EngineIOServer(this.Option = Option);
            Server.OnConnection(OnConnection);

            AckManager.SetTimeout(Option.PingTimeout);
        }

        public SocketIOServer Start()
        {
            Server.Start();

            return this;
        }

        public SocketIOServer Stop()
        {
            Server.Stop();

            return this;
        }

        public override void Dispose()
        {
            Stop();
        }

        private void OnConnection(EngineIOSocket EngineIOSocket)
        {
            SocketIOSocket Socket = new SocketIOSocket(EngineIOSocket, this);

            SimpleMutex.Lock(ClientMutex, () =>
            {
                _Clients.Add(Socket);

                Socket.On(SocketIOEvent.DISCONNECT, () =>
                {
                    SimpleMutex.Lock(ClientMutex, () =>
                    {
                        _Clients.Remove(Socket);
                    });
                });

                Socket.Emit(SocketIOPacket.CreateConnectionPacket());
                ConnectionHandlerManager.Emit(SocketIOEvent.CONNECTION, Socket);
            });
        }
    }
}
