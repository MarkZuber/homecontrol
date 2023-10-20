using System.Threading.Tasks;

namespace HomeControl.Web.Devices;

public interface IEpsonNetworkProjector
{
    Task TurnOnAsync();
    Task TurnOffAsync();
}