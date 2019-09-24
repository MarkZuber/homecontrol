using System.Diagnostics.CodeAnalysis;
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

        [SuppressMessage("AsyncUsage.CSharp.Naming", "UseAsyncSuffix", Justification = "Override of uncontrolled API.")]
        protected override Task Handle(
            AlexaActivityRequest request,
            CancellationToken cancellationToken)
        {
            return _alexaActivityService.ExecuteActivityForAlexaMessageAsync(request.AlexaMessage, cancellationToken);
        }
    }
}
