using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace HomeControl.Web.Devices.Epson;

class SocketConnection : IDisposable
{
    private readonly Socket _socket;
    private readonly IPEndPoint _remoteEndPoint;
    private bool _disposedValue;

    public SocketConnection(string ipAddress, int defaultPort, SocketType socketType, ProtocolType protocolType)
    {
        try
        {
            _socket = new Socket(AddressFamily.InterNetwork, socketType, protocolType);

            string[] addressAndPort = ipAddress.Split(':');
            int port = addressAndPort.Length > 1 ? Convert.ToInt32(addressAndPort[1], 10) : defaultPort;

            IPAddress remoteIPAddress = IPAddress.Parse(addressAndPort[0]);
            _remoteEndPoint = new IPEndPoint(remoteIPAddress, port);
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Cannot setup connection for {_remoteEndPoint.Address}:{_remoteEndPoint.Port} {_socket.ProtocolType}\n\n{e.Message}");
        }
    }

    public bool Connect()
    {
        try
        {
            _socket.Connect(_remoteEndPoint);
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Cannot connect to device at {_remoteEndPoint.Address}:{_remoteEndPoint.Port} {_socket.ProtocolType}\n\n{e.Message}");
        }

        return _socket.Connected;
    }

    public void SendData(byte[] data)
    {
        try
        {
            _socket.Send(data);
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Cannot send Command: {data} to device at {_remoteEndPoint.Address}:{_remoteEndPoint.Port} {_socket.ProtocolType}\n\n{e.Message}");
        }
    }

    public string ReceiveData()
    {
        string reply = "";
        try
        {
            byte[] buffer = new byte[100];
            _socket.Receive(buffer);
            reply = System.Text.Encoding.UTF8.GetString(buffer).TrimEnd('\0');
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Failed to recieve expected data from device at {_remoteEndPoint.Address}:{_remoteEndPoint.Port} {_socket.ProtocolType}\n\n{e.Message}");
        }

        return reply;
    }

    public void CloseSocket()
    {
        Dispose();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _socket?.Close();
                _socket?.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}