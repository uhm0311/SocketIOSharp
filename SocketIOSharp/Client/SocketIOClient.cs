using Newtonsoft.Json.Linq;
using SocketIOSharp.Packet.Ack;
using System;
using WebSocketSharp;

namespace SocketIOSharp.Client
{
    public partial class SocketIOClient : IDisposable
    {
        private readonly ConnctionData ConnectionInformation = new ConnctionData();
        private WebSocket Client = null;

        public string URI
        {
            get
            {
                return string.Format
                (
                    "{0}://{1}:{2}/socket.io/?EIO=4&transport=websocket", 
                    ConnectionInformation.Scheme, 
                    ConnectionInformation.Host, 
                    ConnectionInformation.Port
                );
            }
        }

        public bool JsonOnly { get; set; }
        public bool AutoReconnect { get; set; }

        public bool IsAlive
        {
            get
            {
                return Client?.IsAlive ?? false;
            }
        }

        private SocketIOAckManager AckManager = null;
        public bool UseAckTimeout 
        {
            get 
            {
                return AckManager?.UseAckTimeout ?? false;
            }
            set 
            {
                if (AckManager == null)
                {
                    AckManager = new SocketIOAckManager() { AutoRemove = true };
                }

                if (value)
                {
                    AckManager.StartTimer();
                }
                else
                {
                    AckManager.StopTimer();
                }
            }
        }

        public SocketIOClient(Scheme Scheme, string Host, int Port, bool JsonOnly = false, bool AutoReconnect = false, bool UseAckTimeout = false)
        {
            Initialize(Scheme, Host, Port, JsonOnly, AutoReconnect, UseAckTimeout);
        }

        private void Initialize(Scheme Scheme, string Host, int Port, bool JsonOnly, bool AutoReconnect, bool UseAckTimeout)
        {
            ConnectionInformation.Scheme = Scheme;
            ConnectionInformation.Host = Host;
            ConnectionInformation.Port = Port;

            this.JsonOnly = JsonOnly;
            this.AutoReconnect = AutoReconnect;
            this.UseAckTimeout = UseAckTimeout;

            Initialize();
        }

        private void Initialize()
        {
            Client = new WebSocket(URI);

            Client.OnOpen += OnWebsocketOpen;
            Client.OnClose += OnWebsocketClose;
            Client.OnMessage += OnWebsocketMessage;
            Client.OnError += OnWebsocketError;
        }

        public void Connect()
        {
            if (Client == null)
            {
                Initialize();
            }
                
            Client.Connect();
        }

        public void Close()
        {
            Client?.Close();
            Client = null;

            AckManager?.Dispose();
            AckManager = null;

            StopHeartbeat();
            Reconstructor.Dispose();
        }

        public void Dispose()
        {
            Close();
        }

        public delegate void EventAction(JToken[] Data);
        public delegate void AckAction(JToken[] Data, EventAction Callback);

        public enum Scheme
        {
            ws,
            wss,
        };

        private class ConnctionData
        {
            internal Scheme Scheme = Scheme.ws;
            internal string Host = string.Empty;
            internal int Port = 0;

            internal string SocketID = string.Empty;
            internal int PingInterval = 0;
            internal int PingTimeout = 0;
        }
    }
}