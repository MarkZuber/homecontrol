using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HomeControl.Web.Activities;

namespace HomeControl.Web.Services
{
    /// <summary>
    /// Handles the mapping between streamdeck keys and their activity, the button images for the activity, etc
    /// </summary>
    public class StreamDeckActivityService : IStreamDeckActivityService
    {
        public const int NumKeys = 15;

        private readonly IActivityService _activityService;
        private readonly StreamDeckActivityServiceConfig _config;

        public StreamDeckActivityService(
            IActivityService activityService,
            StreamDeckActivityServiceConfig config)
        {
            if (config.KeyInfos.Count != NumKeys)
            {
                throw new ArgumentException($"There must be {NumKeys} keyinfos in the config", nameof(config));
            }

            _activityService = activityService;
            _config = config;
        }

        public StreamDeckKeyInfo GetKeyInfoAtIndex(int keyIndex)
        {
            return _config.KeyInfos[keyIndex];
        }

        public async Task ExecuteActivityAtIndex(int keyIndex, CancellationToken cancellationToken)
        {
            await _activityService
                .ExecuteActivityAsync(_config.KeyInfos[keyIndex].Key, cancellationToken)
                .ConfigureAwait(false);
        }

    }
}
