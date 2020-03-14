using Newtonsoft.Json.Linq;
using SocketIOSharp.Packet;

namespace SocketIOSharp.Client
{
    partial class SocketIOClient
    {
        private void HandleEnginePacket(SocketIOPacket Packet, bool IsBinary)
        {
            if (Packet != null && !(JsonOnly && IsBinary))
            {
                switch (Packet.EnginePacketType)
                {
                    case EngineIOPacketType.OPEN:
                        HandleOpen((JObject)Packet.JsonData);
                        break;

                    case EngineIOPacketType.CLOSE:
                        HandleClose();
                        break;

                    case EngineIOPacketType.PING:
                        HandlePing();
                        break;

                    case EngineIOPacketType.PONG:
                        HandlePong();
                        break;

                    case EngineIOPacketType.MESSAGE:
                        HandleMessage(Packet, IsBinary);
                        break;

                    default:
                        HandleEtcEnginePacket();
                        break;
                }
            }
        }

        private void HandleOpen(JToken JsonData)
        {
            if (JsonData != null)
            {
                ConnectionInformation.SocketID = JsonData["sid"].ToString();
                ConnectionInformation.PingInterval = int.Parse(JsonData["pingInterval"].ToString());
                ConnectionInformation.PingTimeout = int.Parse(JsonData["pingTimeout"].ToString());

                AckManager.SetTimeout(ConnectionInformation.PingTimeout);
                StartHeartbeatTimers();
            }
        }

        private void HandleClose()
        {
            this.Close();
        }

        private void HandlePing()
        {
            this.Emit(SocketIOPacket.Factory.CreatePongPacket());
        }

        private void HandlePong()
        {
            Pong++;
        }

        private void HandleMessage(SocketIOPacket Packet, bool IsBinary = false)
        {
            if (Packet != null)
                HandleSocketPacket(Packet, IsBinary);
        }

        private void HandleEtcEnginePacket()
        {
        }
    }
}
