using MediatR;

namespace HomeControl.Web.Mediatr
{
    public class AlexaActivityRequest : IRequest
    {
        public AlexaActivityRequest(string msg)
        {
            AlexaMessage = msg.ToLowerInvariant();
        }

        public string AlexaMessage { get; }
    }
}
