using SocketIOSharp.Common.Manager;

namespace SocketIOSharp.Abstract
{
    partial class SocketIOConnection
    {
        protected SocketIOAckManager AckManager = null;

        internal SocketIOConnection(bool JsonOnly = false, bool UseAckTimeout = false)
        {
            this.JsonOnly = JsonOnly;
            this.UseAckTimeout = UseAckTimeout;
        }
    }
}
