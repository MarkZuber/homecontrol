namespace HomeControl.Web.Devices.Meta;

public class Theater : ITheater
{
    public Theater(IDenonNetworkReceiver receiver, ISonyNetworkProjector projector, IEpsonNetworkProjector epsonProjector)
    {
        Receiver = receiver;
        Projector = projector;
        EpsonProjector = epsonProjector;
    }

    public IDenonNetworkReceiver Receiver { get; }

    public ISonyNetworkProjector Projector { get; }

    public IEpsonNetworkProjector EpsonProjector { get; }
}
