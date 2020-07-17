using System;

namespace SocketIOSharp.Common
{
    internal class SocketIOException : Exception
    {
        internal SocketIOException(string message) : base(message) 
        { 
        }

        internal SocketIOException(string message, Exception innerException) : base(message, innerException) 
        { 
        }
    }
}
