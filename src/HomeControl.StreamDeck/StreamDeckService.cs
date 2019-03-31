using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HomeControl.StreamDeck.Api;
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
        private Timer _timer;
        private readonly Random _random = new Random();
        private readonly IStreamDeckApi _streamDeckApi;

        public StreamDeckService(
            ILogger<StreamDeckService> logger,
            IOptions<StreamDeckConfig> config,
            IStreamDeckApi streamDeckApi)
        {
            _logger = logger;
            _config = config;
            _streamDeckApi = streamDeckApi;
            _streamDeckController = StreamDeckFactory.CreateDeck();
            _streamDeckController.KeyPressed += OnStreamDeckKeyPressed;
        }

        private void OnStreamDeckKeyPressed(object sender, StreamDeckKeyChangedEventArgs e)
        {
            _logger.LogInformation($"StreamDeck Key pressed: {e.KeyIndex} -> {e.KeyOn}");
            if (e.KeyOn)
            {
                _streamDeckApi.PressKey(e.KeyIndex);
            }
        }

        private void DoWork(object state)
        {
            for (int i = 0; i < _streamDeckController.NumKeys; i++)
            {
                //var bytes = new byte[3];
                //_random.NextBytes(bytes);
                //_streamDeckController.FillColor(i, bytes[0], bytes[1], bytes[2]);

                try
                {
                    var imageBytes = _streamDeckApi.GetImageForKey(i);
                    using (var ms = new MemoryStream(imageBytes))
                    {
                        _streamDeckController.SetImage(i, ms);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failure calling GetImageForKey");
                }
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting StreamDeckService");
            _streamDeckController.FillAllKeysWithColor(0, 255, 0);

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping StreamDeckService.");
            _timer?.Change(Timeout.Infinite, 0);
            _streamDeckController.ClearAllKeys();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _logger.LogInformation("Disposing....");
            _timer?.Dispose();
            _streamDeckController.KeyPressed += OnStreamDeckKeyPressed;
            _streamDeckController.Dispose();
        }
    }
}
