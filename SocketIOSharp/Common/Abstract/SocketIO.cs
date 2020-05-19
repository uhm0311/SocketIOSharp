using SocketIOSharp.Common.Manager;
using System;

namespace SocketIOSharp.Common.Abstract
{
    public abstract partial class SocketIO<TChildClass> : IDisposable where TChildClass : SocketIO<TChildClass>
    {
        protected SocketIOAckManager AckManager = new SocketIOAckManager() { AutoRemove = true };

        protected SocketIO()
        {
            if (!(this is TChildClass))
            {
                throw new ArgumentException(string.Format(@"'{0}' does not match with '{1}'.", GetType().Name, typeof(TChildClass).Name));
            }
        }

        public abstract void Dispose();
    }
}
