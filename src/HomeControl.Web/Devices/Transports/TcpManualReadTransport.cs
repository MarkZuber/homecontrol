using System;
using System.Threading;
using System.Threading.Tasks;

namespace HomeControl.Web.Devices.Transports
{
    public class TcpManualReadTransport : AbstractTcpTransport,
                                          ITcpManualReadTransport
    {
        public async Task ConnectAsync(string ipAddress, int port, byte[] receiveTerminatorBytes)
        {
            await InitializeAsync(ipAddress, port, receiveTerminatorBytes, null);
        }

        public async Task ConnectAsync(string ipAddress, int port, byte[] receiveTerminatorBytes, Action<object> longRunningConnectionTask)
        {
            await InitializeAsync(ipAddress, port, receiveTerminatorBytes, longRunningConnectionTask);
        }

        public async Task<byte[]> ReceiveMessageAsync(TimeSpan timeout, CancellationToken cancellationToken)
        {
            EnsureConnected();

            const int MaxBufferSize = 1000;
            var buf = new byte[MaxBufferSize]; // todo: make maxBufferSize configurable

            NetworkStream.ReadTimeout = (int)timeout.TotalMilliseconds;
            var numBytesRead = await NetworkStream.ReadAsync(buf, 0, MaxBufferSize, cancellationToken);

            var finalBuf = new byte[numBytesRead];
            if (numBytesRead > 0)
            {
                Array.Copy(buf, finalBuf, numBytesRead);
            }

            return finalBuf;
        }
    }
}
