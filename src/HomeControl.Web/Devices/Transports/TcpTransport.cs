using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace HomeControl.Web.Devices.Transports
{
    public class TcpTransport : AbstractTcpTransport,
                                ITcpTransport
    {
        public async Task ConnectAsync(string ipAddress, int port, params byte[] receiveTerminatorBytes)
        {
            await InitializeAsync(ipAddress, port, receiveTerminatorBytes, CheckForMessages);
        }

        public event EventHandler<TransportMessageReceivedEventArgs> TransportMessageReceived;

        private void CheckForMessages(object ctx)
        {
            while (!CancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    var finalBuffer = ReadMessageBytes();

                    if (!CancellationTokenSource.IsCancellationRequested)
                    {
                        TransportMessageReceived?.Invoke(this, new TransportMessageReceivedEventArgs(finalBuffer));
                    }

                    /*
                    for (var b = (byte)_networkStream.ReadByte(); i < 135 && !hashTerminators.Contains(b); b = (byte)_networkStream.ReadByte())
                    {
                        char c = (char)b;
                        buf[i++] = b;
                    }

                    var finalBuffer = new byte[i];
                    Array.Copy(buf, 0, finalBuffer, 0, i);
                    TransportMessageReceived?.Invoke(this, new TransportMessageReceivedArgs(finalBuffer));
                    */
                }
                catch (AggregateException)
                {
                    TransitionState(ConnectionState.Reconnecting);
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

        private byte[] ReadMessageBytes()
        {
            EnsureConnected();

            var ms = new MemoryStream();

            const int MaxBufferSize = 1000;
            var buf = new byte[MaxBufferSize]; // todo: make maxBufferSize configurable

            while (ms.Length < MaxBufferSize && !CancellationTokenSource.IsCancellationRequested)
            {
                var val = NetworkStream.ReadByte();
                if (val < 0)
                {
                    Task.Delay(100).Wait();
                }
                else
                {
                    var b = (byte)val;

                    if (IsTerminatorByte(b))
                    {
                        break;
                    }

                    ms.WriteByte(b);
                }
            }

            var finalBuffer = ms.ToArray();
            return finalBuffer;
        }
    }
}
