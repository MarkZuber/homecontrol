using System.Threading;
using System.Threading.Tasks;
using HomeControl.Web.Services;
using MediatR;

namespace HomeControl.Web.Mediatr
{
    public class AlexaActivityRequestHandler : AsyncRequestHandler<AlexaActivityRequest>
    {
        private readonly IAlexaActivityService _alexaActivityService;

        public AlexaActivityRequestHandler(IAlexaActivityService alexaActivityService)
        {
            _alexaActivityService = alexaActivityService;
        }

        protected override Task Handle(
            AlexaActivityRequest request,
            CancellationToken cancellationToken)
        {
            return _alexaActivityService.ExecuteActivityForAlexaMessage(request.AlexaMessage, cancellationToken);
        }
    }
}
