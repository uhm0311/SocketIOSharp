namespace SocketIOSharp.Server.Client
{
    partial class SocketIOSocket
    {
        protected override void Emit(string Data)
        {
            Socket.Send(Data);
        }

        protected override void Emit(byte[] RawData)
        {
            Socket.Send(RawData);
        }
    }
}
