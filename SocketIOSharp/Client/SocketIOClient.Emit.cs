using Newtonsoft.Json.Linq;
using SocketIOSharp.Common;
using SocketIOSharp.Common.Action;
using SocketIOSharp.Common.Packet;

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

        public void Emit(JToken Event, SocketIOAction.Event Callback = null)
        {
            Emit(Event, Arguments(Callback));
        }

        public void Emit(JToken Event, JToken Data, SocketIOAction.Event Callback = null)
        {
            Emit(Event, Arguments(Data, Callback));
        }

        public void Emit(JToken Event, params object[] Arguments)
        {
            if (Event != null)
            {
                JArray JsonArray = new JArray();
                SocketIOAction.Event Callback = null;
                int ArgumentsCount = Arguments.Length;

                if (ArgumentsCount > 0 && Arguments[Arguments.Length - 1] is SocketIOAction.Event)
                {
                    ArgumentsCount--;
                    Callback = (SocketIOAction.Event)Arguments[Arguments.Length - 1];
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
