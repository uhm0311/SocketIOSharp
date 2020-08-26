namespace SocketIOSharp.Client
{
    partial class SocketIOClient
    {
        protected override SocketIOClient Emit(string Data)
        {
            Client?.Send(Data);

            return this;
        }

        protected override SocketIOClient Emit(byte[] RawData)
        {
            Client?.Send(RawData);

            return this;
        }
    }
}
