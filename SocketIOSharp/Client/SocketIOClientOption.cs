using EngineIOSharp.Client;
using EngineIOSharp.Common.Enum;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace SocketIOSharp.Client
{
    public class SocketIOClientOption : EngineIOClientOption
    {
        private int _ReconnectionDelay;
        private int _ReconnectionDelayMax;
        private double _RandomizationFactor;

        public bool Reconnection { get; set; }
        public ulong ReconnectionAttempts { get; set; }

        public int ReconnectionDelay
        {
            get
            {
                return _ReconnectionDelay;
            }
            set
            {
                _ReconnectionDelay = Math.Max(0, value);
            }
        }
        public int ReconnectionDelayMax
        {
            get
            {
                return _ReconnectionDelayMax;
            }
            set
            {
                _ReconnectionDelayMax = Math.Max(ReconnectionDelay, value);
            }
        }
        public double RandomizationFactor
        {
            get
            {
                return _RandomizationFactor;
            }
            set
            {
                _RandomizationFactor = Math.Max(0, Math.Min(1, value));
            }
        }

        /// <summary>
        /// Options for Socket.IO client.
        /// </summary>
        /// <param name="Scheme">Scheme to connect to.</param>
        /// <param name="Host">Host to connect to.</param>
        /// <param name="Port">Port to connect to.</param>
        /// <param name="PolicyPort">Port the policy server listens on.</param>
        /// <param name="Path">Path to connect to.</param>
        /// <param name="Reconnection">Whether to reconnect to server after Engine.IO client is closed or not.</param>
        /// <param name="ReconnectionAttempts">Number of reconnection attempts before giving up.</param>
        /// <param name="ReconnectionDelay">How ms to initially wait before attempting a new reconnection.</param>
        /// <param name="ReconnectionDelayMax">Maximum amount of time to wait between reconnections. Each attempt increases the <see cref="ReconnectionDelay"/> by 2x along with a randomization.</param>
        /// <param name="RandomizationFactor">0 &lt;= <see cref="RandomizationFactor"/> &lt;= 1.</param>
        /// <param name="Query">Parameters that will be passed for each request to the server.</param>
        /// <param name="Upgrade">Whether the client should try to upgrade the transport.</param>
        /// <param name="RemeberUpgrade">Whether the client should bypass normal upgrade process when previous websocket connection is succeeded.</param>
        /// <param name="ForceBase64">Forces base 64 encoding for transport.</param>
        /// <param name="WithCredentials">Whether to include credentials such as cookies, authorization headers, TLS client certificates, etc. with polling requests.</param>
        /// <param name="TimestampRequests">Whether to add the timestamp with each transport request. Polling requests are always stamped.</param>
        /// <param name="TimestampParam">Timestamp parameter.</param>
        /// <param name="Polling">Whether to include polling transport.</param>
        /// <param name="PollingTimeout">Timeout for polling requests in milliseconds.</param>
        /// <param name="WebSocket">Whether to include websocket transport.</param>
        /// <param name="WebSocketSubprotocols">List of <see href="https://developer.mozilla.org/en-US/docs/Web/API/WebSockets_API/Writing_WebSocket_servers#Subprotocols">websocket subprotocols</see>.</param>
        /// <param name="ExtraHeaders">Headers that will be passed for each request to the server.</param>
        /// <param name="ClientCertificates">The collection of security certificates that are associated with each request.</param>
        /// <param name="ClientCertificateSelectionCallback">Callback used to select the certificate to supply to the server.</param>
        /// <param name="ServerCertificateValidationCallback">Callback method to validate the server certificate.</param>
        public SocketIOClientOption(EngineIOScheme Scheme, string Host, ushort Port, ushort PolicyPort = 843, string Path = "/socket.io", bool Reconnection = true, ulong ReconnectionAttempts = ulong.MaxValue, int ReconnectionDelay = 1000, int ReconnectionDelayMax = 5000, double RandomizationFactor = 0.5, IDictionary<string, string> Query = null, bool Upgrade = true, bool RemeberUpgrade = false, bool ForceBase64 = false, bool WithCredentials = true, bool? TimestampRequests = null, string TimestampParam = "t", bool Polling = true, int PollingTimeout = 0, bool WebSocket = true, string[] WebSocketSubprotocols = null, IDictionary<string, string> ExtraHeaders = null, X509CertificateCollection ClientCertificates = null, LocalCertificateSelectionCallback ClientCertificateSelectionCallback = null, RemoteCertificateValidationCallback ServerCertificateValidationCallback = null) : base(Scheme, Host, Port, PolicyPort, Path, PolishQuery(Query), Upgrade, RemeberUpgrade, ForceBase64, WithCredentials, TimestampRequests, TimestampParam, Polling, PollingTimeout, WebSocket, WebSocketSubprotocols, ExtraHeaders, ClientCertificates, ClientCertificateSelectionCallback, ServerCertificateValidationCallback)
        {
            this.Reconnection = Reconnection;
            this.ReconnectionAttempts = ReconnectionAttempts;

            this.ReconnectionDelay = ReconnectionDelay;
            this.ReconnectionDelayMax = ReconnectionDelayMax;
            this.RandomizationFactor = RandomizationFactor;
        }

        private static IDictionary<string, string> PolishQuery(IDictionary<string, string> Query)
        {
            Query = new Dictionary<string, string>(Query ?? new Dictionary<string, string>());

            if (!Query.ContainsKey("EIO"))
            {
                Query["EIO"] = "4";
            }

            return Query;
        }
    }
}
