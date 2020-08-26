using EngineIOSharp.Client;
using EngineIOSharp.Common.Enum;
using EngineIOSharp.Common.Packet;
using SocketIOSharp.Common;
using SocketIOSharp.Common.Abstract.Connection;
using System;
using System.Threading;

namespace SocketIOSharp.Client
{
    public partial class SocketIOClient : SocketIOConnection<SocketIOClient>
    {
        private static readonly Random Random = new Random();
        private DateTime LastPing = DateTime.UtcNow;

        private EngineIOClient Client = null;
        private ulong ReconnectionAttempts = 0;
        private bool Closing = false;

        public SocketIOClientOption Option { get; private set; }

        public override EngineIOReadyState ReadyState => Client?.ReadyState ?? EngineIOReadyState.CLOSED;

        public SocketIOClient(SocketIOClientOption Option)
        {
            this.Option = Option;
        }

        public SocketIOClient Connect()
        {
            if (Client == null)
            {
                ReconnectionAttempts = 0;
                Closing = false;

                void Connect()
                {
                    Client = new EngineIOClient(Option);
                    Client.OnOpen(() =>
                    {
                        AckManager.SetTimeout(Client.Handshake.PingTimeout);
                        AckManager.StartTimer();

                        if (ReconnectionAttempts > 0)
                        {
                            CallEventHandler(Event.RECONNECT, ReconnectionAttempts);
                            ReconnectionAttempts = 0;
                        }
                    });

                    Client.OnMessage(OnPacket);
                    Client.OnClose((Exception) =>
                    {
                        OnDisconnect(Exception);

                        if (ReconnectionAttempts == 0)
                        {
                            CallEventHandler(Event.CONNECT_ERROR, new SocketIOException("Connect error", Exception).ToString());
                        }
                        else
                        {
                            CallEventHandler(Event.RECONNECT_ERROR, new SocketIOException("Reconnect error", Exception).ToString());
                        }

                        if (!Closing && Option.Reconnection && ReconnectionAttempts < Option.ReconnectionAttempts)
                        {
                            ReconnectionAttempts++;

                            ThreadPool.QueueUserWorkItem((_) =>
                            {
                                int Factor = (int)(Option.ReconnectionDelay * Option.RandomizationFactor);
                                int Delay = Random.Next(Option.ReconnectionDelay - Factor, Option.ReconnectionDelay + Factor + 1);

                                Thread.Sleep(Delay);
                                Option.ReconnectionDelay = Math.Min(Option.ReconnectionDelayMax, Option.ReconnectionDelay * 2);

                                CallEventHandler(Event.RECONNECT_ATTEMPT);
                                Connect();

                                CallEventHandler(Event.RECONNECTING, ReconnectionAttempts);
                            });
                        }
                        else if (ReconnectionAttempts >= Option.ReconnectionAttempts)
                        {
                            CallEventHandler(Event.RECONNECT_FAILED);
                        }
                    });

                    Client.On(EngineIOClient.Event.PACKET_CREATE, (Argument) =>
                    {
                        if ((Argument as EngineIOPacket).Type == EngineIOPacketType.PING)
                        {
                            CallEventHandler(Event.PING);
                            LastPing = DateTime.UtcNow;
                        }
                    });

                    Client.On(EngineIOClient.Event.PACKET, (Argument) =>
                    {
                        if ((Argument as EngineIOPacket).Type == EngineIOPacketType.PONG)
                        {
                            CallEventHandler(Event.PONG, DateTime.UtcNow.Subtract(LastPing).TotalMilliseconds);
                        }
                    });

                    Client.Connect();
                }

                Connect();
            }

            return this;
        }

        public override SocketIOClient Close()
        {
            Closing = true;
            Client?.Close();

            return this;
        }
    }
}