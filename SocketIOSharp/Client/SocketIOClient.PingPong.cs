using SocketIOSharp.Packet;
using System.Timers;

namespace SocketIOSharp.Client
{
    partial class SocketIOClient
    {
        private ulong Pong = 0;

        private Timer Heartbeat = null;
        private Timer HeartbeatListener = null;

        private void StartHeartbeatTimers()
        {
            StartHeartbeat();
            StartHeartbeatListener();
        }

        private void StartHeartbeat()
        {
            Heartbeat = new Timer(ConnectionInformation.PingInterval);
            Heartbeat.Elapsed += (sender, e) => this.Emit(SocketIOPacket.Factory.CreatePingPacket());

            Heartbeat.AutoReset = true;
            Heartbeat.Start();
        }

        private void StartHeartbeatListener()
        {
            HeartbeatListener = new Timer(ConnectionInformation.PingTimeout);
            HeartbeatListener.Elapsed += (sender, e) =>
            {
                if (Pong > 0)
                    Pong = 0;
                else this.Close();
            };

            HeartbeatListener.AutoReset = true;
            HeartbeatListener.Start();
        }

        private void StopHeartbeatTimers()
        {
            StopHeartbeat();
            StopHeartbeatListener();
        }

        private void StopHeartbeat()
        {
            if (Heartbeat != null)
            {
                Heartbeat.Stop();
                Heartbeat = null;
            }
        }

        private void StopHeartbeatListener()
        {
            if (HeartbeatListener != null)
            {
                HeartbeatListener.Stop();
                HeartbeatListener = null;
            }
        }
    }
}