using EmitterSharp;
using Newtonsoft.Json.Linq;
using SocketIOSharp.Common.Packet;
using SocketIOSharp.Common.Packet.Binary.Constructors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SocketIOSharp.Common.Abstract.Connection
{
    partial class SocketIOConnection<TChildClass>
    {
        protected readonly Reconstructor Reconstructor = new Reconstructor();

        private readonly SocketIOEventHandlerManager EventHandlerManager = new SocketIOEventHandlerManager();
        private readonly SocketIOAckEventHandlerManager AckEventHandlerManager = new SocketIOAckEventHandlerManager();

        public TChildClass On(JToken Event, Action Callback)
        {
            EventHandlerManager.On(Event, Callback);

            return this as TChildClass;
        }

        public TChildClass On(JToken Event, Action<JToken[]> Callback)
        {
            EventHandlerManager.On(Event, Callback);

            return this as TChildClass;
        }

        public TChildClass Once(JToken Event, Action Callback)
        {
            EventHandlerManager.Once(Event, Callback);

            return this as TChildClass;
        }

        public TChildClass Once(JToken Event, Action<JToken[]> Callback)
        {
            EventHandlerManager.Once(Event, Callback);

            return this as TChildClass;
        }

        public TChildClass Off(JToken Event, Action Callback)
        {
            EventHandlerManager.Off(Event, Callback);

            return this as TChildClass;
        }

        public TChildClass Off(JToken Event, Action<JToken[]> Callback)
        {
            EventHandlerManager.Off(Event, Callback);

            return this as TChildClass;
        }

        public TChildClass On(JToken Event, Action<SocketIOAckEvent> Callback)
        {
            AckEventHandlerManager.On(Event, Callback);

            return this as TChildClass;
        }

        public TChildClass Once(JToken Event, Action<SocketIOAckEvent> Callback)
        {
            AckEventHandlerManager.Once(Event, Callback);

            return this as TChildClass;
        }

        public TChildClass Off(JToken Event, Action<SocketIOAckEvent> Callback)
        {
            AckEventHandlerManager.Off(Event, Callback);

            return this as TChildClass;
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
                    CallAckEventHandler(Event, Packet.ID, Data);
                }
            }
        }

        private void CallAckEventHandler(JToken Event, int PacketID, params JToken[] Data)
        {
            if (Event != null)
            {
                Action<JToken[]> Callback = null;

                if (PacketID >= 0)
                {
                    Callback = (AckData) =>
                    {
                        Emit(SocketIOPacket.CreateAckPacket(new JArray(AckData), PacketID));
                    };
                }

                AckEventHandlerManager.Emit(Event, new SocketIOAckEvent(Data, Callback));
            }
        }

        protected void CallEventHandler(JToken Event, params JToken[] Data)
        {
            if (Event != null)
            {
                EventHandlerManager.Emit(Event, Data);
            }
        }

        private class SocketIOEventHandlerManager : Emitter<SocketIOEventHandlerManager, JToken, JToken[]>
        {
            
        }

        private class SocketIOAckEventHandlerManager : Emitter<SocketIOAckEventHandlerManager, JToken, SocketIOAckEvent>
        {

        }
    }
}
