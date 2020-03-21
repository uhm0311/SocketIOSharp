using Newtonsoft.Json.Linq;
using SocketIOSharp.Common;
using SocketIOSharp.Common.Action;
using SocketIOSharp.Common.Packet;

namespace SocketIOSharp.Client
{
    partial class SocketIOClient
    {
        internal override void Emit(SocketIOPacket Packet)
        {
            if (IsAlive && Packet != null)
            {
                object Encoded = SocketIOPacket.Encode(Packet);

                if (Packet.IsBinary)
                {
                    Client.Send((byte[])Encoded);
                }
                else
                {
                    Client.Send((string)Encoded);
                }

                foreach (SocketIOPacket Attachment in Packet.Attachments)
                {
                    Emit(Attachment);
                }
            }
        }
    }
}
