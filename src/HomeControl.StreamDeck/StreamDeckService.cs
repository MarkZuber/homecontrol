using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Zube.StreamDeck;

namespace HomeControl.StreamDeck
{
    public class StreamDeckService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IOptions<StreamDeckConfig> _config;
        private readonly IStreamDeckController _streamDeckController;

        public StreamDeckService(ILogger<StreamDeckService> logger, IOptions<StreamDeckConfig> config)
        {
            _logger = logger;
            _config = config;
            _streamDeckController = StreamDeckFactory.CreateDeck();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting StreamDeckService");
            _streamDeckController.FillAllKeysWithColor(0, 255, 0);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping StreamDeckService.");
            _streamDeckController.ClearAllKeys();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _logger.LogInformation("Disposing....");
            _streamDeckController.Dispose();
        }
    }
}
