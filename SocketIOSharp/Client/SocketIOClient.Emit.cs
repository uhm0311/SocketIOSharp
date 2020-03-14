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
                ArgumentsCount--;

            object[] Result = new object[ArgumentsCount];
            for (int i = 0; i < ArgumentsCount; i++)
                Result[i] = Arguments[i];

            return Result;
        }

        public void Emit(JToken Event, Action<JToken[]> AckAction = null)
        {
            this.Emit(Event, Arguments(AckAction));
        }

        public void Emit(JToken Event, JToken Data, Action<JToken[]> AckAction = null)
        {
            this.Emit(Event, Arguments(Data, AckAction));
        }

        public void Emit(JToken Event, params object[] Arguments)
        {
            if (Event != null)
            {
                JArray JsonArray = new JArray();
                Action<JToken[]> AckAction = null;
                int ArgumentsCount = Arguments.Length;

                if (ArgumentsCount > 0 && Arguments[Arguments.Length - 1] != null && Arguments[Arguments.Length - 1] is Action<JToken[]>)
                {
                    ArgumentsCount--;
                    AckAction = (Action<JToken[]>)Arguments[Arguments.Length - 1];
                }

                JsonArray.Add(Event);
                for (int i = 0; i < ArgumentsCount; i++)
                {
                    JToken Data;

                    try { Data = JToken.FromObject(Arguments[i]); }
                    catch { Data = JValue.CreateNull(); }

                    JsonArray.Add(Data);
                }

                this.Emit(SocketIOPacket.Factory.CreateEventPacket(JsonArray, AckManager.CreateAck(AckAction), JsonOnly));
            }
        }

        private void Emit(SocketIOPacket Packet)
        {
            if (this.Client != null && Packet != null)
            {
                object Encoded = SocketIOPacket.Encode(Packet);

                if (Packet.IsBinary)
                    Client.Send((byte[])Encoded);
                else Client.Send((string)Encoded);

                foreach (SocketIOPacket Attachment in Packet.Attachments)
                    this.Emit(Attachment);
            }
        }
    }
}
