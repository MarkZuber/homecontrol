using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControl.Web.Devices.Meta
{
    public class Theater : ITheater
    {
        public Theater(IDenonNetworkReceiver receiver, ISonyNetworkProjector projector)
        {
            Receiver = receiver;
            Projector = projector;
        }

        public IDenonNetworkReceiver Receiver { get; }

        public ISonyNetworkProjector Projector { get; }
    }
}
