using Newtonsoft.Json.Linq;
using SocketIOSharp.Client;
using System;

namespace SocketIOSharp.Common.Packet.Binary.Constructors
{
    internal class Reconstructor : Constructor
    {
        public override void SetPacket(SocketIOPacket ConstructeePacket)
        {
            if (ConstructeePacket != null)
            {
                base.SetPacket(ConstructeePacket);

                ConstructorAction EnqueueAction = new ConstructorAction((Data) => ConstructeeTokens.Enqueue(Data));
                Tuple<Condition, ConstructorAction>[] ConditionalActions = new Tuple<Condition, ConstructorAction>[]
                {
                    new Tuple<Condition, ConstructorAction>(IsPlaceholder, EnqueueAction),
                };

                DFS(ConstructeePacket.JsonData, ConditionalActions);

                if (ConstructeePacket.Attachments.Count != ConstructeeTokenCount)
                {
                    throw new SocketIOClientException("Attachment count is not match to placeholder count. " + this);
                }
            }
        }

        public SocketIOPacket Reconstruct(byte[] BinaryData)
        {
            if (ConstructeeTokenCount > 0)
            {
                JToken Parent = base.DequeueConstructeeTokenParent(out object Key);
                ConstructeePacket.Attachments.Dequeue();

                Parent[Key] = BinaryData;
            }

            if (ConstructeeTokenCount == 0)
            {
                return ConstructeePacket;
            }
            else
            {
                return null;
            }
        }

        public override string ToString()
        {
            return string.Format("Reconstructor: {0}", base.ToString());
        }
    }
}
