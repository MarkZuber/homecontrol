using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HomeControl.StreamDeck.Api;
using HomeControl.StreamDeck.Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Zube.StreamDeck;

namespace HomeControl.StreamDeck
{
    public class StreamDeckService : TaskHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IOptions<StreamDeckConfig> _config;
        private readonly IStreamDeckController _streamDeckController;
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
            if (e.KeyOn)
            {
                _logger.LogInformation($"StreamDeck Key pressed: {e.KeyIndex}");
                _streamDeckApi.PressKey(e.KeyIndex);
            }
        }

        public void Dispose()
        {
            _logger.LogInformation("Disposing....");
            _streamDeckController.KeyPressed += OnStreamDeckKeyPressed;
            _streamDeckController.Dispose();
        }

        private async Task UpdateImageForKeyAsync(int keyIndex)
        {
            var imageBytes = await _streamDeckApi.GetImageForKeyAsync(keyIndex);
            using (var ms = new MemoryStream(imageBytes))
            {
                _streamDeckController.SetImage(keyIndex, ms);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            //var bytes = new byte[3];
            //_random.NextBytes(bytes);
            //_streamDeckController.FillColor(i, bytes[0], bytes[1], bytes[2]);

            _streamDeckController.FillAllKeysWithColor(0, 255, 0);

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var tasks = new List<Task>();
                    for (int i = 0; i < _streamDeckController.NumKeys; i++)
                    {
                        tasks.Add(UpdateImageForKeyAsync(i));
                    }

                    await Task.WhenAll(tasks.ToArray());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failure calling GetImageForKey");
                throw;
            }
            finally
            {
                _streamDeckController.ClearAllKeys();
            }
        }
    }
}
