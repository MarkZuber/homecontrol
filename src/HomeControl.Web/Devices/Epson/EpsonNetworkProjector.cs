using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HomeControl.Web.Services;

namespace HomeControl.Web.Devices.Epson
{
    public class EpsonNetworkProjector : IEpsonNetworkProjector
    {
        // https://github.com/nicko88/HTWebRemote/blob/master/HTWebRemote/Devices/Controllers/EpsonControl.cs
        // https://github.com/nicko88/HTWebRemote/blob/master/IP%20Control%20Spec%20Documents/Epson_PJ.pdf

        //initialize connection: ESC/VP.net\x10\x03\x00\x00\x00\x00
        private static readonly byte[] s_init =
            { 0x45, 0x53, 0x43, 0x2F, 0x56, 0x50, 0x2E, 0x6E, 0x65, 0x74, 0x10, 0x03, 0x00, 0x00, 0x00, 0x00 };

        private const int EpsonNetworkPort = 3629;

        private readonly string _ipAddress;

        public EpsonNetworkProjector(INetworkSettings networkSettings)
        {
            _ipAddress = networkSettings.EpsonProjectorAddress;
        }

        public Task TurnOffAsync()
        {
            RunCmd(_ipAddress, "PWR OFF");
            return Task.CompletedTask;
        }

        public Task TurnOnAsync()
        {
            RunCmd(_ipAddress, "PWR ON");
            return Task.CompletedTask;
        }

        private static void RunCmd(string ipAddress, string cmd)
        {
            using var socket = new SocketConnection(
                ipAddress, EpsonNetworkPort, SocketType.Stream, ProtocolType.Tcp);

            if (socket.Connect())
            {
                socket.SendData(s_init);
                Thread.Sleep(100);

                socket.SendData(Encoding.ASCII.GetBytes($"{cmd}\r"));
            }
        }

        private static string Query(string ipAddress, string cmd)
        {
            using var socket = new SocketConnection(
                ipAddress, EpsonNetworkPort, SocketType.Stream, ProtocolType.Tcp);
            string dataResponse = "Error getting value";

            if (socket.Connect())
            {
                socket.SendData(s_init);
                Thread.Sleep(100);

                socket.SendData(Encoding.ASCII.GetBytes($"{cmd}\r"));

                _ = socket.ReceiveData();
                dataResponse = socket.ReceiveData();
            }

            try
            {
                dataResponse = dataResponse.Substring(
                    dataResponse.LastIndexOf('=') + 1,
                    dataResponse.Length - dataResponse.LastIndexOf(':') + 2);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error parsing data: {dataResponse}\n\n{e.Message}");
            }

            return dataResponse;
        }
    }
}
