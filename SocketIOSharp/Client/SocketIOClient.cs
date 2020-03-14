using SocketIOSharp.Packet.Ack;
using System;
using WebSocketSharp;

namespace SocketIOSharp.Client
{
    public partial class SocketIOClient : IDisposable
    {
        private WebSocket Client = null;
        private ConnctionData ConnectionInformation = new ConnctionData();

        public bool JsonOnly { get; set; }
        public bool AutoReconnect { get; set; }

        private SocketIOAckManager AckManager = null;
        public bool UseAckTimeout 
        {
            get 
            {
                if (AckManager != null)
                    return AckManager.UseAckTimeout;
                else return false;
            }
            set 
            {
                if (AckManager == null)
                    AckManager = new SocketIOAckManager() { AutoRemove = true };

                if (value)
                    AckManager.StartTimer();
                else AckManager.StopTimer();
            }
        }

        public SocketIOClient(SocketIOClient.Scheme Scheme, string Host, int Port, bool JsonOnly = false, bool AutoReconnect = false, bool UseAckTimeout = false)
        {
            init(Scheme, Host, Port, JsonOnly, AutoReconnect, UseAckTimeout);
        }

        private void init(SocketIOClient.Scheme Scheme, string Host, int Port, bool JsonOnly, bool AutoReconnect, bool UseAckTimeout)
        {
            string URIString = string.Format("{0}://{1}:{2}/socket.io/?EIO=4&transport=websocket", Scheme, Host, Port);

            this.Client = new WebSocket(URIString);

            this.Client.OnOpen += OnWebsocketOpen;
            this.Client.OnClose += OnWebsocketClose;
            this.Client.OnMessage += OnWebsocketMessage;
            this.Client.OnError += OnWebsocketError;

            this.ConnectionInformation.Scheme = Scheme;
            this.ConnectionInformation.Host = Host;
            this.ConnectionInformation.Port = Port;

            this.JsonOnly = JsonOnly;
            this.AutoReconnect = AutoReconnect;
            this.UseAckTimeout = UseAckTimeout;
        }

        public void Connect()
        {
            if (this.Client == null)
                init(this.ConnectionInformation.Scheme, this.ConnectionInformation.Host, this.ConnectionInformation.Port, this.JsonOnly, this.AutoReconnect, this.UseAckTimeout);
            this.Client.Connect();
        }

        public void Close()
        {
            if (this.Client != null && this.Client.IsAlive)
            {
                this.Client.Close();
                this.Client = null;
            }

            if (this.AckManager != null)
            {
                this.AckManager.Dispose();
                this.AckManager = null;
            }

            this.StopHeartbeatTimers();
            this.Reconstructor.Dispose();
        }

        public void Dispose()
        {
            this.Close();
        }

        public enum Scheme
        {
            ws,
            wss,
        };

        private class ConnctionData
        {
            internal SocketIOClient.Scheme Scheme = Scheme.ws;
            internal string Host = string.Empty;
            internal int Port = 0;

            internal string SocketID = string.Empty;
            internal int PingInterval = 0;
            internal int PingTimeout = 0;
        }
    }
}