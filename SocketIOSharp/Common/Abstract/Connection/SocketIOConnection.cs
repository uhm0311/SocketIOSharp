using EngineIOSharp.Common.Enum;

namespace SocketIOSharp.Common.Abstract.Connection
{
    public abstract partial class SocketIOConnection : SocketIO
    {
        public abstract EngineIOReadyState ReadyState { get; }

        public abstract void Close();

        public override void Dispose()
        {
            Close();
        }
    }
}
