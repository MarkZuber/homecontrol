using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControl.Web.Devices.Denon
{
    public class DenonNetworkReceiver : IDenonNetworkReceiver
    {
        public Task TurnOffAsync()
        {
            return Task.CompletedTask;
        }

        public Task TurnOnAsync()
        {
            return Task.CompletedTask;
        }
    }
}
