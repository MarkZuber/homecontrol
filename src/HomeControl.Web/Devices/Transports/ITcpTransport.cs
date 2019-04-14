using System;

namespace HomeControl.Web.Devices.Transports
{
    public interface ITcpTransport : IBaseTcpTransport
    {
        event EventHandler<TransportMessageReceivedEventArgs> TransportMessageReceived;
    }
}
