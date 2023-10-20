using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HomeControl.Web.Activities;
using Serilog;

namespace HomeControl.Web.Services
{
    /// <summary>
    /// Handles the mapping between streamdeck keys and their activity, the button images for the activity, etc
    /// </summary>
    public class StreamDeckActivityService : IStreamDeckActivityService
    {
        public const int NumKeys = 15;

        private readonly ILogger _logger;
        private readonly IActivityService _activityService;
        private readonly StreamDeckActivityServiceConfig _config;

        public StreamDeckActivityService(
            ILogger logger,
            IActivityService activityService,
            StreamDeckActivityServiceConfig config)
        {
            _logger = logger;
            if (config.KeyInfos.Count != NumKeys)
            {
                string msg = $"There must be {NumKeys} keyinfos in the config";
                _logger.Error(msg);
                throw new ArgumentException(msg, nameof(config));
            }
            _activityService = activityService;
            _config = config;
        }

        public StreamDeckKeyInfo GetKeyInfoAtIndex(int keyIndex)
        {
            return _config.KeyInfos[keyIndex];
        }

        public async Task ExecuteActivityAtIndexAsync(int keyIndex, CancellationToken cancellationToken)
        {
            _logger.Information($"ExecuteActivityAtIndexAsync: {keyIndex}");
            await _activityService
                .ExecuteActivityAsync(_config.KeyInfos[keyIndex].Key, cancellationToken)
                .ConfigureAwait(false);
        }

    }
}
