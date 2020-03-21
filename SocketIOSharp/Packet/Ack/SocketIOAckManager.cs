using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Timers;
using static SocketIOSharp.Client.SocketIOClient;

namespace SocketIOSharp.Packet.Ack
{
    internal class SocketIOAckManager
    {
        private readonly Timer AckTimer = new Timer() { AutoReset = true };

        public bool UseAckTimeout { get; private set; }
        public const int MinTimeout = 10;
        private double TimeoutInMillis = 60 * 1000;

        private readonly List<SocketIOAck> AckList = new List<SocketIOAck>();
        private readonly object AckMutex = new object();

        public bool AutoRemove { get; set; }

        public SocketIOAckManager()
        {
            SetTimeout(TimeoutInMillis);
            UseAckTimeout = false;

            AckTimer.Elapsed += (sender, e) =>
            {
                lock (AckMutex)
                {
                    for (int i = 0; UseAckTimeout && i < AckList.Count; i++)
                    {
                        if (AckList[i] != null && DateTime.UtcNow.Subtract(AckList[i].RequestedTime).TotalMilliseconds > TimeoutInMillis)
                        {
                            AckList[i] = null;
                        }
                    }
                }
            };
        }

        public void Dispose()
        {
            StopTimer();
            AckList.Clear();
        }

        public void StartTimer()
        {
            if (!UseAckTimeout)
            {
                UseAckTimeout = true;
                AckTimer.Start();
            }
        }

        public void StopTimer()
        {
            if (UseAckTimeout)
            {
                UseAckTimeout = false;
                AckTimer.Stop();
            }
        }

        public void SetTimeout(double TimeoutInMillis)
        {
            if (TimeoutInMillis >= MinTimeout)
            {
                AckTimer.Interval = (this.TimeoutInMillis = TimeoutInMillis) / MinTimeout;
            }
        }

        public SocketIOAck CreateAck(EventAction Callback = null)
        {
            if (Callback != null)
            {
                lock (AckMutex)
                {
                    for (int i = 0; true; i++)
                    {
                        if (i < AckList.Count)
                        {
                            if (AckList[i] == null)
                            {
                                return (AckList[i] = new SocketIOAck(i, Callback));
                            }
                        }
                        else
                        {
                            SocketIOAck Ack = new SocketIOAck(AckList.Count, Callback);
                            AckList.Add(Ack);

                            return Ack;
                        }
                    }
                }
            }
            else
            {
                return null;
            }
        }

        public void Invoke(int PacketID, params JToken[] Data)
        {
            lock (AckMutex)
            {
                if (AckList.Count > PacketID && AckList[PacketID] != null)
                {
                    AckList[PacketID].Invoke(Data);

                    if (AutoRemove)
                    {
                        AckList[PacketID] = null;
                    }
                }
            }
        }

        public void Remove(int PacketID)
        {
            lock (AckMutex)
            {
                if (AckList.Count > PacketID)
                {
                    AckList[PacketID] = null;
                }
            }
        }
    }
}
