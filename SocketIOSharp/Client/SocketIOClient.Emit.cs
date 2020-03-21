using Newtonsoft.Json.Linq;
using SocketIOSharp.Packet;
using System;

namespace SocketIOSharp.Client
{
    partial class SocketIOClient
    {
        private object[] Arguments(params object[] Arguments)
        {
            int ArgumentsCount = Arguments.Length;

            if (Arguments[Arguments.Length - 1] == null)
            {
                ArgumentsCount--;
            }

            object[] Result = new object[ArgumentsCount];

            for (int i = 0; i < ArgumentsCount; i++)
            {
                Result[i] = Arguments[i];
            }

            return Result;
        }

        public void Emit(JToken Event, EventAction Callback = null)
        {
            Emit(Event, Arguments(Callback));
        }

        public void Emit(JToken Event, JToken Data, EventAction Callback = null)
        {
            Emit(Event, Arguments(Data, Callback));
        }

        public void Emit(JToken Event, params object[] Arguments)
        {
            if (Event != null)
            {
                JArray JsonArray = new JArray();
                EventAction Callback = null;
                int ArgumentsCount = Arguments.Length;

                if (ArgumentsCount > 0 && Arguments[Arguments.Length - 1] is EventAction)
                {
                    ArgumentsCount--;
                    Callback = (EventAction)Arguments[Arguments.Length - 1];
                }

                JsonArray.Add(Event);

                for (int i = 0; i < ArgumentsCount; i++)
                {
                    JToken Data;

                    try { Data = JToken.FromObject(Arguments[i]); }
                    catch { Data = JValue.CreateNull(); }

                    JsonArray.Add(Data);
                }

                Emit(SocketIOPacket.Factory.CreateEventPacket(JsonArray, AckManager.CreateAck(Callback), JsonOnly));
            }
        }

        private void Emit(SocketIOPacket Packet)
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
