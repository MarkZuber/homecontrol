using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControl.Web.Devices
{
    public interface ITheater
    {
        IDenonNetworkReceiver Receiver { get; }
        ISonyNetworkProjector Projector { get; }
        IEpsonNetworkProjector EpsonProjector {get;}
    }
}
