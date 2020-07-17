namespace SocketIOSharp.Server
{
    partial class SocketIOServer
    {
        protected override SocketIOServer Emit(string Data)
        {
            Server.Broadcast(Data);

            return this;
        }

        protected override SocketIOServer Emit(byte[] RawData)
        {
            Server.Broadcast(RawData);

            return this;
        }
    }
}
