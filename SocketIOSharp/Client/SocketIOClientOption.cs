using EngineIOSharp.Client;
using EngineIOSharp.Common.Enum;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace SocketIOSharp.Client
{
    public class SocketIOClientOption : EngineIOClientOption
    {
        public bool UseAckTimeout { get; private set; }
        public bool AutoReconnect { get; private set; }

        /// <summary>
        /// Options for Socket.IO client.
        /// </summary>
        /// <param name="Scheme">Scheme to connect to.</param>
        /// <param name="Host">Host to connect to.</param>
        /// <param name="Port">Port to connect to.</param>
        /// <param name="PolicyPort">Port the policy server listens on.</param>
        /// <param name="UseAckTimeout">Whether to use ack timeout or not.</param>
        /// <param name="AutoReconnect">Whether to reconnect to server after Engine.IO client is closed or not.</param>
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
        public SocketIOClientOption(EngineIOScheme Scheme, string Host, ushort Port, ushort PolicyPort = 843, bool UseAckTimeout = false, bool AutoReconnect = true, IDictionary<string, string> Query = null, bool Upgrade = true, bool RemeberUpgrade = false, bool ForceBase64 = false, bool WithCredentials = true, bool? TimestampRequests = null, string TimestampParam = "t", bool Polling = true, int PollingTimeout = 0, bool WebSocket = true, string[] WebSocketSubprotocols = null, IDictionary<string, string> ExtraHeaders = null, X509CertificateCollection ClientCertificates = null, LocalCertificateSelectionCallback ClientCertificateSelectionCallback = null, RemoteCertificateValidationCallback ServerCertificateValidationCallback = null) : base(Scheme, Host, Port, PolicyPort, "/socket.io", PolishQuery(Query), Upgrade, RemeberUpgrade, ForceBase64, WithCredentials, TimestampRequests, TimestampParam, Polling, PollingTimeout, WebSocket, WebSocketSubprotocols, ExtraHeaders, ClientCertificates, ClientCertificateSelectionCallback, ServerCertificateValidationCallback)
        {
            this.UseAckTimeout = UseAckTimeout;
            this.AutoReconnect = AutoReconnect;
        }

        private static IDictionary<string, string> PolishQuery(IDictionary<string, string> Query)
        {
            Query = new Dictionary<string, string>(Query ?? new Dictionary<string, string>());

            if (!Query.ContainsKey("EIO") || Query["EIO"].Equals("3"))
            {
                Query["EIO"] = "4";
            }

            return Query;
        }
    }
}
