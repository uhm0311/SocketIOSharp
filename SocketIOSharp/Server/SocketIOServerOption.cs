using EngineIOSharp.Common;
using EngineIOSharp.Server;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using WebSocketSharp.Net;
using WebSocketSharp.Net.WebSockets;

namespace SocketIOSharp.Server
{
    public class SocketIOServerOption : EngineIOServerOption
    {
        /// <summary>
        /// Options for Socket.IO server.
        /// </summary>
        /// <param name="Port">Port to listen.</param>
        /// <param name="Secure">Whether to secure connections.</param>
        /// <param name="PingTimeout">How many ms without a pong packet to consider the connection closed.</param>
        /// <param name="PingInterval">How many ms before sending a new ping packet.</param>
        /// <param name="UpgradeTimeout">How many ms before an uncompleted transport upgrade is cancelled.</param>
        /// <param name="Polling">Whether to accept polling transport.</param>
        /// <param name="WebSocket">Whether to accept websocket transport.</param>
        /// <param name="AllowUpgrade">Whether to allow transport upgrade.</param>
        /// <param name="SetCookie">Whether to use cookie.</param>
        /// <param name="SIDCookieName">Name of sid cookie.</param>
        /// <param name="Cookies">Configuration of the cookie that contains the client sid to send as part of handshake response headers. This cookie might be used for sticky-session.</param>
        /// <param name="AllowHttpRequest">A function that receives a given handshake or upgrade http request as its first parameter, and can decide whether to continue or not.</param>
        /// <param name="AllowWebSocket">A function that receives a given handshake or upgrade websocket connection as its first parameter, and can decide whether to continue or not.</param>
        /// <param name="InitialData">An optional packet which will be concatenated to the handshake packet emitted by Engine.IO.</param>
        /// <param name="ServerCertificate">The certificate used to authenticate the server.</param>
        /// <param name="ClientCertificateValidationCallback">Callback used to validate the certificate supplied by the client.</param>
        public SocketIOServerOption(ushort Port, bool Secure = false, ulong PingTimeout = 5000, ulong PingInterval = 25000, ulong UpgradeTimeout = 10000, bool Polling = true, bool WebSocket = true, bool AllowUpgrade = true, bool SetCookie = true, string SIDCookieName = "io", IDictionary<string, string> Cookies = null, Action<HttpListenerRequest, Action<EngineIOException>> AllowHttpRequest = null, Action<WebSocketContext, Action<EngineIOException>> AllowWebSocket = null, object InitialData = null, X509Certificate2 ServerCertificate = null, RemoteCertificateValidationCallback ClientCertificateValidationCallback = null) : base(Port, "/socket.io", Secure, PingTimeout, PingInterval, UpgradeTimeout, Polling, WebSocket, AllowUpgrade, SetCookie, SIDCookieName, Cookies, AllowHttpRequest, AllowWebSocket, InitialData, ServerCertificate, ClientCertificateValidationCallback)
        {
            
        }
    }
}
