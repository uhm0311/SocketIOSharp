namespace SocketIOSharp.Server
{
    partial class SocketIOServer
    {
        protected override void Emit(string Data)
        {
            Server.Broadcast(Data);
        }

        protected override void Emit(byte[] RawData)
        {
            Server.Broadcast(RawData);
        }
    }
}
