using EngineIOSharp.Common.Packet;
using Newtonsoft.Json.Linq;
using SocketIOSharp.Common;
using SocketIOSharp.Common.Packet;
using SocketIOSharp.Common.Packet.Binary.Constructors;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SocketIOSharp.Common.Abstract.Connection
{
    partial class SocketIOConnection
    {
        protected bool Closed = false;

        protected readonly Reconstructor Reconstructor = new Reconstructor();
        private readonly ConcurrentDictionary<JToken, List<Action<JToken[]>>> EventHandlers = new ConcurrentDictionary<JToken, List<Action<JToken[]>>>();
        private readonly ConcurrentDictionary<JToken, List<Action<JToken[], Action<JToken[]>>>> AckHandlers = new ConcurrentDictionary<JToken, List<Action<JToken[], Action<JToken[]>>>>();

        protected void OnPacket(EngineIOPacket Packet)
        {
            OnPacket(SocketIOPacket.Decode(Packet));
        }

        private void OnPacket(SocketIOPacket Packet)
        {
            switch (Packet.Type)
            {
                case SocketIOPacketType.CONNECT:
                    OnConnect();
                    break;

                case SocketIOPacketType.DISCONNECT:
                    OnDisconnect();
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

        private void OnConnect()
        {
            CallEventHandler(SocketIOEvent.CONNECTION);
        }

        protected void OnDisconnect()
        {
            if (!Closed)
            {
                Closed = true;

                CallEventHandler(SocketIOEvent.DISCONNECT);
                Close();
            }
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
            CallEventHandler(SocketIOEvent.ERROR, Packet?.JsonData?.ToString() ?? string.Empty);
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

        public void On(JToken Event, Action<JToken[]> Callback)
        {
            if (Event != null)
            {
                if (!EventHandlers.ContainsKey(Event))
                {
                    EventHandlers.TryAdd(Event, new List<Action<JToken[]>>());
                }

                EventHandlers[Event].Add(Callback);
            }
        }

        public void Off(JToken Event, Action<JToken[]> Callback)
        {
            if (Event != null && EventHandlers.ContainsKey(Event))
            {
                EventHandlers[Event].Remove(Callback);
            }
        }

        public void On(JToken Event, Action<JToken[], Action<JToken[]>> Callback)
        {
            if (Event != null)
            {
                if (!AckHandlers.ContainsKey(Event))
                {
                    AckHandlers.TryAdd(Event, new List<Action<JToken[], Action<JToken[]>>>());
                }

                AckHandlers[Event].Add(Callback);
            }
        }

        public void Off(JToken Event, Action<JToken[], Action<JToken[]>> Callback)
        {
            if (Event != null && AckHandlers.ContainsKey(Event))
            {
                AckHandlers[Event].Remove(Callback);
            }
        }

        private void CallHandler(SocketIOPacket Packet)
        {
            if (Packet != null && Packet.JsonData != null)
            {
                Queue<JToken> Temp = new Queue<JToken>(((JArray)Packet.JsonData).ToArray());

                if (Temp.Count > 0)
                {
                    JToken Event = Temp.Dequeue();
                    JToken[] Data = Temp.ToArray();

                    CallEventHandler(Event, Data);
                    CallAckHandler(Event, Packet.ID, Data);
                }
            }
        }

        private void CallAckHandler(JToken Event, int PacketID, params JToken[] Data)
        {
            if (Event != null && AckHandlers.ContainsKey(Event))
            {
                foreach (Action<JToken[], Action<JToken[]>> AckHandler in AckHandlers[Event])
                {
                    Action<JToken[]> Callback = null;

                    if (PacketID >= 0)
                    {
                        Callback = (AckData) =>
                        {
                            Emit(SocketIOPacket.CreateAckPacket(new JArray(AckData), PacketID));
                        };
                    }

                    AckHandler(Data, Callback);
                }
            }
        }

        private void CallEventHandler(JToken Event, params JToken[] Data)
        {
            if (Event != null && EventHandlers.ContainsKey(Event))
            {
                foreach (Action<JToken[]> EventHandler in EventHandlers[Event])
                {
                    EventHandler(Data);
                }
            }
        }
    }
}
