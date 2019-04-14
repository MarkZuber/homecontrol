using System;
using System.Threading;
using System.Threading.Tasks;

namespace HomeControl.Web.Devices.Transports
{
    public interface IBaseTcpTransport
    {
        CancellationTokenSource CancellationTokenSource { get; }

        string IpAddress { get; }
        int Port { get; }

        Task ConnectAsync(string ipAddress, int port, params byte[] receiveTerminatorBytes);

        Task DisconnectAsync();

        Task SendMessageByteAsync(byte[] messageBytes);
        Task SendMessageAsync(string requestMessage);

        event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;
    }
}
