using System;

namespace HomeControl.Web.Devices.Transports
{
    public enum ConnectionState
    {
        Disconnected = 0,
        Connecting = 1,
        Connected = 2,
        Disconnecting = 3,
        Reconnecting = 4
    }

    public class ConnectionStateChangedEventArgs : EventArgs
    {
        public ConnectionStateChangedEventArgs(ConnectionState connectionState)
        {
            ConnectionState = connectionState;
        }

        public ConnectionState ConnectionState { get; }
    }
}
