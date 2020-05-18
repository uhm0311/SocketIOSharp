namespace SocketIOSharp.Client
{
    partial class SocketIOClient
    {
        protected override void Emit(string Data)
        {
            Client.Send(Data);
        }

        protected override void Emit(byte[] RawData)
        {
            Client.Send(RawData);
        }
    }
}
