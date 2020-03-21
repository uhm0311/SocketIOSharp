using SocketIOSharp.Common.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketIOSharp.Abstract
{
    public abstract partial class SocketIOConnection : IDisposable
    {
        public bool JsonOnly { get; set; }

        public bool UseAckTimeout
        {
            get
            {
                return AckManager?.UseAckTimeout ?? false;
            }
            set
            {
                if (AckManager == null)
                {
                    AckManager = new SocketIOAckManager() { AutoRemove = true };
                }

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

        public void Dispose()
        {
            Close();
        }

        public abstract void Close();
    }
}
