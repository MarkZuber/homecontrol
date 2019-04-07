using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControl.Web.Devices
{
    public interface IDenonNetworkReceiver
    {
        Task TurnOnAsync();
        Task TurnOffAsync();
        Task VolumeUpAsync();
        Task VolumeDownAsync();
        Task ToggleMuteAsync();
        Task SelectXboxInputAsync();
        Task SelectPs4InputAsync();
        Task SelectAppleTvInputAsync();
        Task SelectFireTvInputAsync();
    }
}
