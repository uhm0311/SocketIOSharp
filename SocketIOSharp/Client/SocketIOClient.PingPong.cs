using SocketIOSharp.Packet;
using System.Timers;

namespace SocketIOSharp.Client
{
    partial class SocketIOClient
    {
        private ulong Pong = 0;

        private Timer Heartbeat = null;
        private Timer HeartbeatListener = null;

        private void StartHeartbeat()
        {
            if (Heartbeat == null)
            {
                Heartbeat = new Timer(ConnectionInformation.PingInterval);
                Heartbeat.Elapsed += (sender, e) =>
                {
                    Emit(SocketIOPacket.Factory.CreatePingPacket());
                    StartHeartbeatListener();
                };

                Heartbeat.AutoReset = true;
                Heartbeat.Start();
            }
        }

        private void StartHeartbeatListener()
        {
            if (HeartbeatListener == null)
            {
                HeartbeatListener = new Timer(ConnectionInformation.PingTimeout);
                HeartbeatListener.Elapsed += (sender, e) =>
                {
                    if (Pong > 0)
                    {
                        Pong = 0;
                    }
                    else
                    {
                        Close();
                    }
                };

                HeartbeatListener.AutoReset = false;
            }

            if (!HeartbeatListener.Enabled)
            {
                HeartbeatListener.Start();
            }
        }

        private void StopHeartbeat()
        {
            Heartbeat?.Stop();
            Heartbeat = null;

            HeartbeatListener?.Stop();
            HeartbeatListener = null;
        }
    }
}