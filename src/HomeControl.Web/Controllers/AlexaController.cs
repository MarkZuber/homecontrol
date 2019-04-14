using HomeControl.Web.Mediatr;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HomeControl.Web.Controllers
{
    [Route("api/[controller]")]
    public class AlexaController : Controller
    {
        private readonly IMediator _mediator;

        public AlexaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET api/<controller>/theater+xbox+on
        [HttpGet("{message}")]
        public string Get(string message)
        {
            _mediator.Publish(new AlexaNotification(message));
            return message;
        }
    }
}
