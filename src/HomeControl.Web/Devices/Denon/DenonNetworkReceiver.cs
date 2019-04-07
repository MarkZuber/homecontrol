using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControl.Web.Devices.Denon
{
    public class DenonNetworkReceiver : IDenonNetworkReceiver
    {
        private readonly IDenonHttpReceiverDevice _receiver;

        public DenonNetworkReceiver(IDenonHttpReceiverDevice receiver)
        {
            _receiver = receiver;
        }

        public Task SelectAppleTvInputAsync()
        {
            _receiver.MainZoneInput = _receiver.AvailableInputs.Mplay;
            return Task.CompletedTask;
        }

        public Task SelectFireTvInputAsync()
        {
            _receiver.MainZoneInput = _receiver.AvailableInputs.Aux2;
            return Task.CompletedTask;
        }

        public Task SelectPs4InputAsync()
        {
            _receiver.MainZoneInput = _receiver.AvailableInputs.Dvd;
            return Task.CompletedTask;
        }

        public Task SelectXboxInputAsync()
        {
            _receiver.MainZoneInput = _receiver.AvailableInputs.Game;
            return Task.CompletedTask;
        }

        public Task ToggleMuteAsync()
        {
            _receiver.MainZoneMute = !_receiver.MainZoneMute;
            return Task.CompletedTask;
        }

        public Task TurnOffAsync()
        {
            _receiver.DevicePower = false;
            return Task.CompletedTask;
        }

        public Task TurnOnAsync()
        {
            _receiver.MainZonePower = true;
            return Task.CompletedTask;
        }

        public Task VolumeDownAsync()
        {
            _receiver.MainZoneVolume -= 5;
            return Task.CompletedTask;
        }

        public Task VolumeUpAsync()
        {
            _receiver.MainZoneVolume += 5;
            return Task.CompletedTask;
        }
    }
}
