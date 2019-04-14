using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using HomeControl.Web.Devices.Transports;

namespace HomeControl.Web.Devices.Sony
{
    public class SonyNetworkProjectorTcpTransportMessageReceivedEventArgs : EventArgs
    {
        public SonyNetworkProjectorTcpTransportMessageReceivedEventArgs(byte[] message)
        {
            Message = message;
        }

        public byte[] Message { get; }
    }

    public class SonyNetworkProjectorTcpTransport : AbstractTcpTransport
    {
        private readonly BlockingCollection<byte[]> _outgoingMessages;

        public SonyNetworkProjectorTcpTransport(BlockingCollection<byte[]> outgoingMessages)
        {
            _outgoingMessages = outgoingMessages;
        }

        public event EventHandler<SonyNetworkProjectorTcpTransportMessageReceivedEventArgs> MessageReceived;

        public async Task ConnectAsync(string ipAddress, int port, byte[] receiveTerminatorBytes)
        {
            await InitializeAsync(ipAddress, port, receiveTerminatorBytes, ExecuteMessageProcessing);
        }

        private void ExecuteMessageProcessing(object obj)
        {
            while (!CancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    var outgoingMessage = _outgoingMessages.Take(CancellationTokenSource.Token);

                    EnsureConnected();

                    Debug.WriteLine($"Sony Outgoing: {BitConverter.ToString(outgoingMessage)}");

                    SendMessageByteAsync(outgoingMessage).ConfigureAwait(false).GetAwaiter().GetResult();
                    var responseMessage = ReceiveMessageAsync(TimeSpan.FromMilliseconds(100), CancellationTokenSource.Token).ConfigureAwait(false).GetAwaiter().GetResult();

                    Debug.WriteLine($"Sony Response: {BitConverter.ToString(responseMessage)}");

                    if (responseMessage.Length > 0)
                    {
                        if (responseMessage[6] == 0)
                        {
                            // error
                            var errorCode1 = responseMessage[10];
                            var errorCode2 = responseMessage[11];
                            Debug.WriteLine($"error: {errorCode1:x2}{errorCode2:x2}");
                        }

                        MessageReceived?.Invoke(this, new SonyNetworkProjectorTcpTransportMessageReceivedEventArgs(responseMessage));
                    }
                }
                catch (SocketException)
                {
                    // _log.Warn(socketEx, $"Socket Exception to IP Address: {IpAddress}");
                    TransitionState(ConnectionState.Reconnecting);
                }
                catch (IOException)
                {
                    // _log.Warn(ioException, $"IO Exception to IP Address: {IpAddress}");
                    TransitionState(ConnectionState.Reconnecting);
                }
            }

            Disconnect();
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
