using System.Threading;
using System.Threading.Tasks;

namespace HomeControl.Web.Services
{
    public interface IAlexaActivityService
    {
        Task ExecuteActivityForAlexaMessage(string alexaMessage, CancellationToken cancellationToken);
    }
}
