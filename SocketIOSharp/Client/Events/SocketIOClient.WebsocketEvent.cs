using SocketIOSharp.Common;
using SocketIOSharp.Common.Packet;
using System;

namespace SocketIOSharp.Client
{
    partial class SocketIOClient
    {
        private void OnWebsocketOpen(object sender, EventArgs e)
        {
        }

        private void OnWebsocketClose(object sender, WebSocketSharp.CloseEventArgs e)
        {
            CallEventHandler(SocketIOEvent.DISCONNECT);

            if (AutoReconnect)
            {
                Connect();
            }
        }

        private void OnWebsocketError(object sender, WebSocketSharp.ErrorEventArgs e)
        {
            Emit(SocketIOEvent.ERROR, e.Message);
            Close();
        }

        private void OnWebsocketMessage(object sender, WebSocketSharp.MessageEventArgs e)
        {
            SocketIOPacket Packet = null;

            if (e.IsText)
            {
                Packet = SocketIOPacket.Decode(e.Data);
            }
            else if (e.IsBinary)
            {
                Packet = SocketIOPacket.Decode(e.RawData);
            }

            HandleEnginePacket(Packet, e.IsBinary);
        }
    }
}
