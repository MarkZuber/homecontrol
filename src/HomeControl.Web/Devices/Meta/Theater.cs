namespace HomeControl.Web.Devices.Meta;

public class Theater : ITheater
{
    public Theater(IDenonNetworkReceiver receiver, IEpsonNetworkProjector epsonProjector)
    {
        Receiver = receiver;
        EpsonProjector = epsonProjector;
    }

    public IDenonNetworkReceiver Receiver { get; }

    public IEpsonNetworkProjector EpsonProjector { get; }
}
