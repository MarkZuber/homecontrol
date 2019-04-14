using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HomeControl.Web.Devices.Transports
{
    public abstract class AbstractTcpTransport
    {
        private readonly HashSet<byte> _hashTerminators = new HashSet<byte>();
        private TcpClient _streamSocket;

        public string IpAddress { get; private set; }
        public int Port { get; private set; }

        public ConnectionState ConnectionState { get; private set; } = ConnectionState.Disconnected;

        protected NetworkStream NetworkStream { get; private set; }

        public CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();
        public event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;

        protected Task InitializeAsync(string ipAddress, int port, byte[] receiveTerminatorBytes, Action<object> longRunningConnectionTask)
        {
            if (_streamSocket != null)
            {
                throw new InvalidOperationException("StreamSocket is already connected");
            }

            IpAddress = ipAddress;
            Port = port;

            foreach (var terminator in receiveTerminatorBytes)
            {
                _hashTerminators.Add(terminator);
            }

            if (longRunningConnectionTask != null)
            {
                try
                {
                    var task = Task.Factory.StartNew(longRunningConnectionTask, CancellationTokenSource.Token, TaskCreationOptions.LongRunning);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("caught ex: " + ex);
                    throw;
                }
                finally
                {
                    Debug.WriteLine("in the finally");
                }
            }

            EnsureConnected();

            return Task.FromResult(0);
        }

        protected bool IsTerminatorByte(byte by)
        {
            return _hashTerminators.Contains(by);
        }

        protected void TransitionState(ConnectionState connectionState)
        {
            ConnectionState = connectionState;
            // we want this to be async and not block here or else it would interrupt the incoming message receive loop.
            Task.Run(() => ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(ConnectionState)));
        }

        public Task DisconnectAsync()
        {
            if (_streamSocket == null)
            {
                throw new InvalidOperationException("StreamSocket is already disconnected");
            }

            Disconnect();
            return Task.FromResult(0);
        }

        protected virtual void Disconnect()
        {
            TransitionState(ConnectionState.Disconnecting);

            try
            {
                _streamSocket?.Close();
                _streamSocket = null;
                CancellationTokenSource.Cancel();
                NetworkStream = null;
            }
            finally
            {
                TransitionState(ConnectionState.Disconnected);
            }
        }

        public async Task SendMessageByteAsync(byte[] messageBytes)
        {
            if (ConnectionState != ConnectionState.Connected)
            {
                throw new InvalidOperationException("Cannot send message.  TcpTransport is not in Connected state");
            }

            await NetworkStream.WriteAsync(messageBytes, 0, messageBytes.Length);
            await NetworkStream.FlushAsync();
        }

        public async Task SendMessageAsync(string requestMessage)
        {
            await SendMessageByteAsync(Encoding.ASCII.GetBytes(requestMessage));
        }

        protected void EnsureConnected()
        {
            if (ConnectionState != ConnectionState.Connected)
            {
                TransitionState(ConnectionState.Connecting);

                _streamSocket = new TcpClient();

                if (!_streamSocket.ConnectAsync(IpAddress, Port).Wait(10000))
                {
                    Debug.WriteLine("connection timeout");
                    throw new SocketException();
                }

                NetworkStream = _streamSocket.GetStream();

                TransitionState(ConnectionState.Connected);
            }
        }
    }
}
