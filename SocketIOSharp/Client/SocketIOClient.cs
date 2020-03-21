using SocketIOSharp.Abstract;
using SocketIOSharp.Common.Manager;
using WebSocketSharp;

namespace SocketIOSharp.Client
{
    public partial class SocketIOClient : SocketIOConnection
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

        public bool AutoReconnect { get; set; }

        public bool IsAlive
        {
            get
            {
                return Client?.IsAlive ?? false;
            }
        }

        public SocketIOClient(Scheme Scheme, string Host, int Port, bool JsonOnly = false, bool UseAckTimeout = false, bool AutoReconnect = false) : base(JsonOnly, UseAckTimeout)
        {
            Initialize(Scheme, Host, Port, AutoReconnect);
        }

        private void Initialize(Scheme Scheme, string Host, int Port, bool AutoReconnect)
        {
            ConnectionInformation.Scheme = Scheme;
            ConnectionInformation.Host = Host;
            ConnectionInformation.Port = Port;

            this.AutoReconnect = AutoReconnect;

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

        public override void Close()
        {
            Client?.Close();
            Client = null;

            AckManager?.Dispose();
            AckManager = null;

            StopHeartbeat();
            Reconstructor.Dispose();
        }

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