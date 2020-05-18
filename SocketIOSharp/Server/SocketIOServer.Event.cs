using EngineIOSharp.Server.Client;
using SimpleThreadMonitor;
using SocketIOSharp.Common;
using SocketIOSharp.Common.Packet;
using SocketIOSharp.Server.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketIOSharp.Server
{
    partial class SocketIOServer
    {
        private readonly List<Action<SocketIOSocket>> ConnectionHandlers = new List<Action<SocketIOSocket>>();
        private readonly object ConnectionHandlerMutex = new object();

        public void OnConnection(Action<SocketIOSocket> Callback)
        {
            if (Callback != null)
            {
                SimpleMutex.Lock(ConnectionHandlerMutex, () =>
                {
                    ConnectionHandlers.Add(Callback);
                });
            }
        }

        private void OnConnection(EngineIOSocket EngineIOSocket)
        {
            SocketIOSocket Socket = new SocketIOSocket(EngineIOSocket, this);
            SimpleMutex.Lock(ClientMutex, () =>
            {
                _Clients.Add(Socket);

                Socket.On(SocketIOEvent.DISCONNECT, (_) =>
                {
                    SimpleMutex.Lock(ClientMutex, () =>
                    {
                        _Clients.Remove(Socket);
                    });
                });

                Socket.Emit(SocketIOPacket.CreateConnectionPacket());
            });

            SimpleMutex.Lock(ConnectionHandlerMutex, () =>
            {
                foreach (Action<SocketIOSocket> Callback in ConnectionHandlers)
                {
                    Callback(Socket);
                }
            });
        }
    }
}
