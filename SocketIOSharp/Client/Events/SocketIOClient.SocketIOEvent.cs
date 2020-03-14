using Newtonsoft.Json.Linq;
using SocketIOSharp.Packet;
using SocketIOSharp.Packet.Binary.Constructors;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SocketIOSharp.Client
{
    partial class SocketIOClient
    {
        private Reconstructor Reconstructor = new Reconstructor();
        private ConcurrentDictionary<JToken, List<Action<JToken[]>>> EventHandlers = new ConcurrentDictionary<JToken, List<Action<JToken[]>>>();
        private ConcurrentDictionary<JToken, List<Action<JToken[], Action<JToken[]>>>> AckHandlers = new ConcurrentDictionary<JToken, List<Action<JToken[], Action<JToken[]>>>>();

        private void HandleSocketPacket(SocketIOPacket Packet, bool IsBinary)
        {
            switch (Packet.SocketPacketType)
            {
                case SocketIOPacketType.CONNECT:
                    HandleConnect();
                    break;

                case SocketIOPacketType.DISCONNECT:
                    HandleDisconnect();
                    break;

                case SocketIOPacketType.EVENT:
                    HandleEvent(Packet);
                    break;

                case SocketIOPacketType.ACK:
                    HandleAck(Packet);
                    break;

                case SocketIOPacketType.ERROR:
                    HandleError(Packet);
                    break;

                case SocketIOPacketType.BINARY_EVENT:
                    HandleBinaryEvent(Packet);
                    break;

                case SocketIOPacketType.BINARY_ACK:
                    HandleBinaryAck(Packet);
                    break;

                default:
                    HandleEtcSocketPacket(Packet, IsBinary);
                    break;
            }
        }

        private void HandleConnect()
        {
            CallEventHandler(Event.CONNECTION);
        }

        private void HandleDisconnect()
        {
            this.Close();
        }

        private void HandleEvent(SocketIOPacket Packet)
        {
            CallHandler(Packet);
        }

        private void HandleAck(SocketIOPacket Packet)
        {
            if (Packet != null && Packet.JsonData != null)
                AckManager.Invoke(Packet.ID, ((JArray)Packet.JsonData).ToArray<JToken>());
        }

        private void HandleError(SocketIOPacket Packet)
        {
            CallEventHandler(Event.ERROR, Packet != null && Packet.JsonData != null ? Packet.JsonData.ToString() : string.Empty);
        }

        private void HandleBinaryEvent(SocketIOPacket Packet)
        {
            HandleBinaryPacket(Packet);
        }

        private void HandleBinaryAck(SocketIOPacket Packet)
        {
            HandleBinaryPacket(Packet);
        }

        private void HandleBinaryPacket(SocketIOPacket Packet)
        {
            if (Packet != null && Reconstructor.ConstructeeTokenCount == 0)
                Reconstructor.SetPacket(Packet);
            else throw new SocketIOClientException(string.Format("Unexpected binary data: {0}. Reconstructor: {1}.", Packet, Reconstructor.OriginalPacket));
        }

        private void HandleEtcSocketPacket(SocketIOPacket Packet, bool IsBinary)
        {
            if (Packet != null && IsBinary)
            {
                if (Reconstructor.ConstructeeTokenCount > 0)
                {
                    SocketIOPacket ReconstructedPacket = Reconstructor.Reconstruct(Packet.BinaryData);

                    if (Reconstructor.ConstructeeTokenCount == 0)
                    {
                        using (Reconstructor)
                        {
                            if (ReconstructedPacket.ID >= 0)
                                HandleAck(ReconstructedPacket);
                            else HandleEvent(ReconstructedPacket);
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
                    EventHandlers.TryAdd(Event, new List<Action<JToken[]>>());

                EventHandlers[Event].Add(Callback);
            }
        }

        public void Off(JToken Event, Action<JToken[]> Callback)
        {
            if (Event != null && EventHandlers.ContainsKey(Event))
                EventHandlers[Event].Remove(Callback);
        }

        public void On(JToken Event, Action<JToken[], Action<JToken[]>> Callback)
        {
            if (Event != null)
            {
                if (!AckHandlers.ContainsKey(Event))
                    AckHandlers.TryAdd(Event, new List<Action<JToken[], Action<JToken[]>>>());

                AckHandlers[Event].Add(Callback);
            }
        }

        public void Off(JToken Event, Action<JToken[], Action<JToken[]>> Callback)
        {
            if (Event != null && AckHandlers.ContainsKey(Event))
                AckHandlers[Event].Remove(Callback);
        }

        private void CallHandler(SocketIOPacket Packet)
        {
            if (Packet != null && Packet.JsonData != null)
            {
                Queue<JToken> Temp = new Queue<JToken>(((JArray)Packet.JsonData).ToArray<JToken>());

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
                foreach (Action<JToken[], Action<JToken[]>> Callback in AckHandlers[Event])
                {
                    Action<JToken[]> AckAction = null;
                    if (PacketID >= 0)
                        AckAction = (AckData) => this.Emit(SocketIOPacket.Factory.CreateAckPacket(new JArray(AckData), PacketID));

                    Callback(Data, AckAction);
                }
            }
        }

        private void CallEventHandler(JToken Event, params JToken[] Data)
        {
            if (Event != null && EventHandlers.ContainsKey(Event))
            {
                foreach (Action<JToken[]> Callback in EventHandlers[Event])
                    Callback(Data);
            }
        }

        public static class Event
        {
            public static readonly string CONNECTION = "connection";
            public static readonly string DISCONNECT = "disconnect";
            public static readonly string ERROR = "error";
        }
    }
}
