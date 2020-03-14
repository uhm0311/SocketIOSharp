using Newtonsoft.Json.Linq;
using SocketIOSharp.Client;
using System;

namespace SocketIOSharp.Packet.Binary.Constructors
{
    internal class Reconstructor : Constructor
    {
        public override void SetPacket(SocketIOPacket ConstructeePacket)
        {
            if (ConstructeePacket != null)
            {
                base.SetPacket(ConstructeePacket);

                Action<JToken> EnqueueAction = new Action<JToken>((Data) => ConstructeeTokens.Enqueue(Data));

                Tuple<Condition, Action<JToken>>[] ConditionalActions = new Tuple<Condition, Action<JToken>>[]
                {
                    new Tuple<Condition, Action<JToken>>(IsPlaceholder, EnqueueAction),
                };

                base.DFS(ConstructeePacket.JsonData, ConditionalActions);

                if (ConstructeePacket.Attachments.Count != ConstructeeTokenCount)
                    throw new SocketIOClientException("Attachment count is not match to placeholder count. " + this);
            }
        }

        public SocketIOPacket Reconstruct(byte[] BinaryData)
        {
            if (ConstructeeTokenCount > 0)
            {
                JToken Parent = base.DequeueConstructeeTokenParent(out object Key);
                base.ConstructeePacket.Attachments.Dequeue();

                Parent[Key] = BinaryData;
            }

            if (ConstructeeTokenCount == 0)
                return base.ConstructeePacket;
            else return null;
        }

        public override string ToString()
        {
            return string.Format("Reconstructor: {0}", base.ToString());
        }
    }
}
