using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
