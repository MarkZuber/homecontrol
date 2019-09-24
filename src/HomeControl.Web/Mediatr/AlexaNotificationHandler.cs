using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace HomeControl.Web.Mediatr
{
    public class AlexaNotificationHandler : INotificationHandler<AlexaNotification>
    {
        private readonly IMediator _mediator;

        public AlexaNotificationHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <inheritdoc />
        [SuppressMessage("AsyncUsage.CSharp.Naming", "UseAsyncSuffix", Justification = "Override of uncontrolled API.")]
        public async Task Handle(AlexaNotification notification, CancellationToken cancellationToken)
        {
            await _mediator.Send(notification.Request, cancellationToken).ConfigureAwait(false);
        }
    }
}
