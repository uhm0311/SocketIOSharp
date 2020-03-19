using Newtonsoft.Json.Linq;
using SocketIOSharp.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SocketIOSharp.Packet.Binary.Constructors
{
    internal abstract class Constructor : IDisposable
    {
        protected delegate bool Condition(JToken JsonData);

        protected static readonly string PLACEHOLDER = "_placeholder";
        protected static readonly string NUM = "num";

        public SocketIOPacket OriginalPacket { get; private set; }
        protected SocketIOPacket ConstructeePacket = null;

        protected Queue<JToken> ConstructeeTokens = new Queue<JToken>();
        public int ConstructeeTokenCount { get { return ConstructeeTokens.Count; } }

        public void Dispose()
        {
            ConstructeeTokens.Clear();
        }

        public virtual void SetPacket(SocketIOPacket ConstructeePacket)
        {
            if (ConstructeePacket != null)
            {
                this.ConstructeePacket = ConstructeePacket;
                OriginalPacket = ConstructeePacket.DeepClone();
            }
        }

        protected void DFS(JToken JsonData, params Tuple<Condition, ConstructorAction>[] ConditionActions)
        {
            if (JsonData != null)
            {
                if (ConditionActions != null)
                {
                    foreach (Tuple<Condition, ConstructorAction> ConditionAction in ConditionActions)
                    {
                        if (ConditionAction != null)
                        {
                            if (ConditionAction.Item1(JsonData))
                            {
                                ConditionAction.Item2(JsonData);
                            }
                        }
                    }
                }

                if (JsonData.HasValues)
                {
                    for (JToken Child = JsonData.First; Child != null; Child = Child.Next)
                    {
                        DFS(Child, ConditionActions);
                    }
                }
            }
        }

        protected JToken DequeueConstructeeTokenParent(out object ConstructeeTokenKey)
        {
            if (ConstructeeTokenCount > 0)
            {
                JToken ConstructeeToken = ConstructeeTokens.Dequeue();
                JToken ConstructeeTokenParent = ConstructeeToken.Parent;

                while (ConstructeeTokenParent.Parent != null && ConstructeeTokenParent.Type == JTokenType.Property)
                {
                    ConstructeeTokenParent = ConstructeeTokenParent.Parent;
                }

                string ConstructeeTokenPath = ConstructeeToken.Path;
                string ConstructeeTokenParentPath = ConstructeeTokenParent.Path;

                int ConstructeeTokenParentPathLength = ConstructeeTokenParentPath.Length;
                if (ConstructeeTokenParentPathLength > 0)
                {
                    ConstructeeTokenParentPathLength++;
                }

                ConstructeeTokenParent = ConstructeePacket.JsonData.SelectToken(ConstructeeTokenParentPath);
                string ConstructeeTokenRelativePath = ConstructeeTokenPath.Substring(ConstructeeTokenParentPathLength);

                if (ConstructeeTokenRelativePath.StartsWith("[") && ConstructeeTokenRelativePath.EndsWith("]"))
                {
                    ConstructeeTokenRelativePath = ConstructeeTokenRelativePath.Substring(1).Substring(0, ConstructeeTokenRelativePath.Length - 2);
                }

                if (ConstructeeTokenParent.Type == JTokenType.Array)
                {
                    ConstructeeTokenKey = int.Parse(ConstructeeTokenRelativePath);
                }
                else if (ConstructeeTokenParent.Type == JTokenType.Object)
                {
                    ConstructeeTokenKey = ConstructeeTokenRelativePath;
                }
                else
                {
                    throw new SocketIOClientException("Invalid placeholder parent type: " + ConstructeeTokenParent.Type);
                }

                return ConstructeeTokenParent;
            }
            else
            {
                return (JToken)(ConstructeeTokenKey = null);
            }
        }

        protected bool IsPlaceholder(JToken JsonData)
        {
            return (JsonData.Type == JTokenType.Object && JsonData.Count() == 2) &&
                (JsonData.First.Type == JTokenType.Property && JsonData.Last.Type == JTokenType.Property) &&
                (JsonData.First.First.Type == JTokenType.Boolean && JsonData.Last.First.Type == JTokenType.Integer) &&
                (JsonData.First.First.Path.Substring(JsonData.First.First.Path.LastIndexOf('.') + 1).Equals(PLACEHOLDER)) &&
                (JsonData.Last.First.Path.Substring(JsonData.Last.First.Path.LastIndexOf('.') + 1).Equals(NUM));
        }

        protected bool IsBytes(JToken JsonData)
        {
            return JsonData.Type == JTokenType.Bytes;
        }

        public override string ToString()
        {
            return string.Format
            (
                "Packet={0}, OriginalPacket={1}, TokenQueue.Count={2}", 
                ConstructeePacket, 
                OriginalPacket, 
                ConstructeeTokens.Count
            );
        }

        protected delegate void ConstructorAction(JToken Data);
    }
}
