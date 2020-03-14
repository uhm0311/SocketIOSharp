using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketIOSharp.Packet
{
    public enum EngineIOPacketType
    {
        UNKNOWN = -1,
        OPEN = 0,
        CLOSE = 1,
        PING = 2,
        PONG = 3,
        MESSAGE = 4,
        UPGRADE = 5,
        NOOP = 6,
    }
}
