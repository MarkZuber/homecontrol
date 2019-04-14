using System;

namespace HomeControl.Web.Devices.Transports
{
    public class TransportMessageReceivedEventArgs : EventArgs
    {
        public TransportMessageReceivedEventArgs(byte[] message)
        {
            Message = message;
        }

        public byte[] Message { get; }
    }
}
