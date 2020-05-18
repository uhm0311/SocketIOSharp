using SocketIOSharp.Common.Manager;
using System;

namespace SocketIOSharp.Common.Abstract
{
    public abstract partial class SocketIO : IDisposable
    {
        protected SocketIOAckManager AckManager = new SocketIOAckManager() { AutoRemove = true };
        public bool UseAckTimeout
        {
            get
            {
                return AckManager?.UseAckTimeout ?? false;
            }
            set
            {
                if (value)
                {
                    AckManager.StartTimer();
                }
                else
                {
                    AckManager.StopTimer();
                }
            }
        }

        public abstract void Dispose();
    }
}
