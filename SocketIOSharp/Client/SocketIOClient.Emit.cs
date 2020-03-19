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

        public void Emit(JToken Event, SocketIOEventAction AckAction = null)
        {
            Emit(Event, Arguments(AckAction));
        }

        public void Emit(JToken Event, JToken Data, SocketIOEventAction AckAction = null)
        {
            Emit(Event, Arguments(Data, AckAction));
        }

        public void Emit(JToken Event, params object[] Arguments)
        {
            if (Event != null)
            {
                JArray JsonArray = new JArray();
                SocketIOEventAction AckAction = null;
                int ArgumentsCount = Arguments.Length;

                if (ArgumentsCount > 0 && Arguments[Arguments.Length - 1] is SocketIOEventAction)
                {
                    ArgumentsCount--;
                    AckAction = (SocketIOEventAction)Arguments[Arguments.Length - 1];
                }

                JsonArray.Add(Event);

                for (int i = 0; i < ArgumentsCount; i++)
                {
                    JToken Data;

                    try { Data = JToken.FromObject(Arguments[i]); }
                    catch { Data = JValue.CreateNull(); }

                    JsonArray.Add(Data);
                }

                Emit(SocketIOPacket.Factory.CreateEventPacket(JsonArray, AckManager.CreateAck(AckAction), JsonOnly));
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
