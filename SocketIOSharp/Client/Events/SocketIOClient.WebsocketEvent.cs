using SocketIOSharp.Packet;
using System;

namespace SocketIOSharp.Client
{
    partial class SocketIOClient
    {
        private void OnWebsocketOpen(object sender, EventArgs e)
        {
            return;
        }

        private void OnWebsocketClose(object sender, WebSocketSharp.CloseEventArgs e)
        {
            this.CallEventHandler(Event.DISCONNECT);

            if (this.AutoReconnect)
                this.Connect();
        }

        private void OnWebsocketError(object sender, WebSocketSharp.ErrorEventArgs e)
        {
            this.Emit(Event.ERROR, e.Message);
            this.Close();
        }

        private void OnWebsocketMessage(object sender, WebSocketSharp.MessageEventArgs e)
        {
            SocketIOPacket Packet = (e.IsText ? SocketIOPacket.Decode(e.Data) : (e.IsBinary ? SocketIOPacket.Decode(e.RawData) : null));
            this.HandleEnginePacket(Packet, e.IsBinary);
        }
    }
}
