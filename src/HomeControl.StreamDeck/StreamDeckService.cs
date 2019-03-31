﻿using System;
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
        private readonly IStreamDeckApi _streamDeckApi;
        private readonly ConcurrentDictionary<int, byte[]> _keyImageCache = new ConcurrentDictionary<int, byte[]>();

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
            // todo: update the service so we can get "normal" and "pressed" button states
            // so we can toggle the button on push instead of just going blank.
            if (e.KeyOn)
            {
                _logger.LogInformation($"StreamDeck Key pressed: {e.KeyIndex}");
                _streamDeckApi.PressKey(e.KeyIndex);
                _streamDeckController.FillColor(e.KeyIndex, 0, 0, 0);
            }
            else
            {
                if (_keyImageCache.TryGetValue(e.KeyIndex, out byte[] cachedImageBytes))
                {
                    UpdateImageFromBytes(e.KeyIndex, cachedImageBytes);
                }
                else
                {
                    RefreshImagesAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                }
            }
        }

        public void Dispose()
        {
            _logger.LogInformation("Disposing....");
            _streamDeckController.ClearAllKeys();
            _streamDeckController.KeyPressed -= OnStreamDeckKeyPressed;
            _streamDeckController.Dispose();
        }

        // todo: move this to the streamdeck hid control library.
        private void UpdateImageFromBytes(int keyIndex, byte[] imageBytes)
        {
            using (var ms = new MemoryStream(imageBytes))
            {
                _streamDeckController.SetImage(keyIndex, ms);
            }
        }

        private async Task UpdateImageForKeyAsync(int keyIndex)
        {
            try
            {
                var imageBytes = await _streamDeckApi.GetImageForKeyAsync(keyIndex);
                _keyImageCache[keyIndex] = imageBytes;
                UpdateImageFromBytes(keyIndex, imageBytes);
            }
            catch (StreamDeckException ex)
            {
                _logger.LogError(ex, $"SetImage ({keyIndex}) failed");
            }
        }

        private async Task RefreshImagesAsync()
        {
            _streamDeckController.ClearAllKeys();

            var tasks = new List<Task>();
            for (int i = 0; i < _streamDeckController.NumKeys; i++)
            {
                tasks.Add(UpdateImageForKeyAsync(i));
            }

            await Task.WhenAll(tasks.ToArray());
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _streamDeckController.FillAllKeysWithColor(0, 255, 0);
            await RefreshImagesAsync();
        }
    }
}
