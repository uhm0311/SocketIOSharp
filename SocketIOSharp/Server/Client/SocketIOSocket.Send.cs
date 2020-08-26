namespace SocketIOSharp.Server.Client
{
    partial class SocketIOSocket
    {
        protected override SocketIOSocket Emit(string Data)
        {
            Socket.Send(Data);

            return this;
        }

        protected override SocketIOSocket Emit(byte[] RawData)
        {
            Socket.Send(RawData);

            return this;
        }
    }
}
