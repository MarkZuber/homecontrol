using System;
using System.Threading;
using System.Threading.Tasks;

namespace HomeControl.Web.Devices.Transports
{
    public interface ITcpManualReadTransport : IBaseTcpTransport
    {
        Task ConnectAsync(string ipAddress, int port, byte[] receiveTerminatorBytes, Action<object> longRunningConnectionTask);

        Task<byte[]> ReceiveMessageAsync(TimeSpan timeout, CancellationToken cancellationToken);
    }
}
