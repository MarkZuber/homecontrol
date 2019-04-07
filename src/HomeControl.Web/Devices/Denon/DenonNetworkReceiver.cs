using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControl.Web.Devices.Denon
{
    public class DenonNetworkReceiver : IDenonNetworkReceiver
    {
        public Task SelectAppleTvInputAsync()
        {
            return Task.CompletedTask;
        }

        public Task SelectFireTvInputAsync()
        {
            return Task.CompletedTask;
        }

        public Task SelectPs4InputAsync()
        {
            return Task.CompletedTask;
        }

        public Task SelectXboxInputAsync()
        {
            return Task.CompletedTask;
        }

        public Task ToggleMuteAsync()
        {
            return Task.CompletedTask;
        }

        public Task TurnOffAsync()
        {
            return Task.CompletedTask;
        }

        public Task TurnOnAsync()
        {
            return Task.CompletedTask;
        }

        public Task VolumeDownAsync()
        {
            return Task.CompletedTask;
        }

        public Task VolumeUpAsync()
        {
            return Task.CompletedTask;
        }
    }
}
