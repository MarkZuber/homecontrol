using System.Threading;
using System.Threading.Tasks;

namespace HomeControl.Web.Services
{
    public class AlexaActivityService : IAlexaActivityService
    {
        private readonly IActivityService _activityService;
        private readonly AlexaActivityServiceConfig _config;

        public AlexaActivityService(
            IActivityService activityService,
            AlexaActivityServiceConfig config)
        {
            _activityService = activityService;
            _config = config;
        }

        public Task ExecuteActivityForAlexaMessageAsync(string alexaMessage, CancellationToken cancellationToken)
        {
            if (_config.AlexaToActivityKeys.TryGetValue(alexaMessage, out string activityKey))
            {
                return _activityService.ExecuteActivityAsync(activityKey, cancellationToken);
            }

            // todo: logging or other indication that we ate the message?
            return Task.CompletedTask;
        }
    }
}
