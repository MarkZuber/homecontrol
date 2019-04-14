using MediatR;

namespace HomeControl.Web.Mediatr
{
    public class AlexaNotification : INotification
    {
        public AlexaNotification(string message)
        {
            Request = new AlexaActivityRequest(message);
        }

        public IRequest<Unit> Request { get; }
    }
}
