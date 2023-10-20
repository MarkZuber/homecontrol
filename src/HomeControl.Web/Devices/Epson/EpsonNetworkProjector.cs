using System;
using System.Threading.Tasks;

namespace HomeControl.Web.Devices.Epson
{
    public class EpsonNetworkProjector : IEpsonNetworkProjector
    {
        private readonly string _ipAddress;

        public EpsonNetworkProjector(string ipAddress)
        {
            _ipAddress = ipAddress;
        }

        public Task TurnOffAsync()
        {
            EpsonProjectorDevice.RunCmd(_ipAddress, "PWR OFF");
            return Task.CompletedTask;
        }

        public Task TurnOnAsync()
        {
            EpsonProjectorDevice.RunCmd(_ipAddress, "PWR ON");
            return Task.CompletedTask;
        }
    }
}
