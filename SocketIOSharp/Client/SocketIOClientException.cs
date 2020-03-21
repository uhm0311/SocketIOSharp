using System;

namespace SocketIOSharp.Client
{
    internal class SocketIOClientException : Exception
    {
        internal SocketIOClientException(string message) : base(message) 
        { 
        }

        internal SocketIOClientException(string message, Exception innerException) : base(message, innerException) 
        { 
        }
    }
}
