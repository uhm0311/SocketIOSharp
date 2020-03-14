using Newtonsoft.Json.Linq;
using SocketIOSharp.Client;
using System;
using System.Collections.Generic;

namespace SocketIOSharp.Packet.Binary.Constructors
{
    internal class Deconstructor : Constructor
    {
        private int PlaceholderCount = 0;

        public override void SetPacket(SocketIOPacket ConstructeePacket)
        {
            if (ConstructeePacket != null)
            {
                base.SetPacket(ConstructeePacket);

                Action<JToken> EnqueueAction = new Action<JToken>((Data) => ConstructeeTokens.Enqueue(Data));
                Action<JToken> CountAction = new Action<JToken>((Data) => PlaceholderCount++);

                Tuple<Condition, Action<JToken>>[] ConditionalActions = new Tuple<Condition, Action<JToken>>[]
                {
                    new Tuple<Condition, Action<JToken>>(IsBytes, EnqueueAction),
                    new Tuple<Condition, Action<JToken>>(IsPlaceholder, CountAction)
                };

                base.DFS(ConstructeePacket.JsonData, ConditionalActions);

                if (PlaceholderCount > 0)
                    throw new SocketIOClientException("Bytes token count is not match to placeholder count. " + this);
            }
        }

        public SocketIOPacket Deconstruct()
        {
            int PlaceholderIndex = 0;
            while (ConstructeeTokenCount > 0)
            {
                object Key;
                JToken Parent = base.DequeueConstructeeTokenParent(out Key);
                base.ConstructeePacket.Attachments.Enqueue(SocketIOPacket.Decode(EngineIOPacketType.MESSAGE, (byte[])Parent[Key]));

                Parent[Key] = new JObject();
                Parent[Key][PLACEHOLDER] = true;
                Parent[Key][NUM] = PlaceholderIndex++;
            }

            return base.ConstructeePacket;
        }

        public override string ToString()
        {
            return string.Format("Deconstructor: {0}, PlaceholderCount={1}", base.ToString(), PlaceholderCount);
        }
    }
}
