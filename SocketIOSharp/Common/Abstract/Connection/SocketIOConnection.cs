using EngineIOSharp.Common.Enum;
using EngineIOSharp.Common.Packet;
using Newtonsoft.Json.Linq;
using SocketIOSharp.Common.Packet;
using System;
using System.Linq;

namespace SocketIOSharp.Common.Abstract.Connection
{
    public abstract partial class SocketIOConnection<TChildClass> : SocketIO<TChildClass> where TChildClass : SocketIOConnection<TChildClass>
    {
        public abstract EngineIOReadyState ReadyState { get; }

        public abstract TChildClass Close();

        public override void Dispose()
        {
            Close();
        }

        protected void OnPacket(EngineIOPacket Packet)
        {
            OnPacket(SocketIOPacket.Decode(Packet));
        }

        private void OnPacket(SocketIOPacket Packet)
        {
            switch (Packet.Type)
            {
                case SocketIOPacketType.CONNECT:
                    OnConnect(Packet);
                    break;

                case SocketIOPacketType.DISCONNECT:
                    Close();
                    break;

                case SocketIOPacketType.EVENT:
                    OnEvent(Packet);
                    break;

                case SocketIOPacketType.ACK:
                    OnAck(Packet);
                    break;

                case SocketIOPacketType.ERROR:
                    OnError(Packet);
                    break;

                case SocketIOPacketType.BINARY_EVENT:
                    OnBinaryEvent(Packet);
                    break;

                case SocketIOPacketType.BINARY_ACK:
                    OnBinaryAck(Packet);
                    break;

                default:
                    OnEtc(Packet);
                    break;
            }
        }

        private void OnConnect(SocketIOPacket Packet)
        {
            CallEventHandler(SocketIOEvent.CONNECTION, Packet.Namespace);
            
            Emit(SocketIOPacket.CreateConnectionPacket(Packet.Namespace));
        }

        protected void OnDisconnect(Exception Exception)
        {
            CallEventHandler(SocketIOEvent.DISCONNECT, new SocketIOException("Socket closed.", Exception).ToString());

            AckManager.Dispose();
            Reconstructor.Dispose();
        }

        private void OnEvent(SocketIOPacket Packet)
        {
            CallHandler(Packet);
        }

        private void OnAck(SocketIOPacket Packet)
        {
            if (Packet?.JsonData != null)
            {
                AckManager.Invoke(Packet.ID, ((JArray)Packet.JsonData).ToArray());
            }
        }

        private void OnError(SocketIOPacket Packet)
        {
            CallEventHandler(SocketIOEvent.ERROR, Packet.Namespace, Packet?.JsonData?.ToString() ?? string.Empty);
        }

        private void OnBinaryEvent(SocketIOPacket Packet)
        {
            OnBinaryPacket(Packet);
        }

        private void OnBinaryAck(SocketIOPacket Packet)
        {
            OnBinaryPacket(Packet);
        }

        private void OnBinaryPacket(SocketIOPacket Packet)
        {
            if (Packet != null && Reconstructor.ConstructeeTokenCount == 0)
            {
                Reconstructor.SetPacket(Packet);
            }
            else
            {
                throw new SocketIOException(string.Format
                (
                    "Unexpected binary data: {0}. Reconstructor: {1}.",
                    Packet,
                    Reconstructor.OriginalPacket
                ));
            }
        }

        private void OnEtc(SocketIOPacket Packet)
        {
            if (Packet != null)
            {
                if (Reconstructor.ConstructeeTokenCount > 0)
                {
                    SocketIOPacket ReconstructedPacket = Reconstructor.Reconstruct(Packet.RawData);

                    if (Reconstructor.ConstructeeTokenCount == 0)
                    {
                        using (Reconstructor)
                        {
                            if (ReconstructedPacket.ID >= 0)
                            {
                                OnAck(ReconstructedPacket);
                            }
                            else
                            {
                                OnEvent(ReconstructedPacket);
                            }
                        }
                    }
                }
            }
        }
    }
}
