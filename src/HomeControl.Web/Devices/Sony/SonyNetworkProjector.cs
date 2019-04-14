using System;
using System.Threading.Tasks;

namespace HomeControl.Web.Devices.Sony
{
    public class SonyNetworkProjector : ISonyNetworkProjector
    {
        private const int SonyNetworkPort = 53484;

        private readonly string _ipAddress;

        public SonyNetworkProjector(string ipAddress)
        {
            _ipAddress = ipAddress;
        }

        public Task TurnOffAsync()
        {
            return ConnectAndExecuteAsync(device => device.DevicePower = false);
        }

        public Task TurnOnAsync()
        {
            return ConnectAndExecuteAsync(device => device.DevicePower = true);
        }

        private async Task ConnectAndExecuteAsync(Action<ISonyProjectorDevice> action)
        {
            using (var device = new SonyProjectorDevice())
            {
                await device.ConnectAsync(_ipAddress, SonyNetworkPort);
                action(device);
                await Task.Delay(300);
            }
        }
    }
}
