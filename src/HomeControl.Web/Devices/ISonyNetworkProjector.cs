using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControl.Web.Devices
{
    public interface ISonyNetworkProjector
    {
        Task TurnOnAsync();
        Task TurnOffAsync();
    }
}
