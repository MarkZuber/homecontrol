using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HomeControl.StreamDeck.Api;
using HomeControl.StreamDeck.Common;
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
        private SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

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

        private async Task UpdateKeyImageAsync(int keyIndex, bool isKeyPressed)
        {
            // Add NumKeys so that the service will get the "pressed" button image instead
            int offsetToAdd = isKeyPressed ? _streamDeckController.NumKeys : 0;
            if (_keyImageCache.TryGetValue(keyIndex + offsetToAdd, out byte[] cachedImageBytes))
            {
                UpdateImageFromBytes(keyIndex, cachedImageBytes);
            }
            else
            {
                await RefreshImagesAsync().ConfigureAwait(false);
            }
        }

        private void OnStreamDeckKeyPressed(object sender, StreamDeckKeyChangedEventArgs e)
        {
            bool locked = _semaphoreSlim.Wait(500);
            try
            {
                // todo: update the service so we can get "normal" and "pressed" button states
                // so we can toggle the button on push instead of just going blank.
                if (e.KeyOn)
                {
                    _logger.LogInformation($"StreamDeck Key pressed: {e.KeyIndex}");

                    try
                    {

                        // todo: get "local" operations we can run from server side button config...
                        switch (e.KeyIndex)
                        {
                        case 11:
                            _streamDeckController.SetBrightness(5);
                            break;
                        case 12:
                            _streamDeckController.SetBrightness(75);
                            break;
                        default:
                            _streamDeckApi.PressKey(e.KeyIndex);
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"StreamDeck Key Press failed");
                    }
                }

                UpdateKeyImageAsync(e.KeyIndex, e.KeyOn).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            finally
            {
                if (locked)
                {
                    _semaphoreSlim.Release();
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
            try
            {
                using (var ms = new MemoryStream(imageBytes))
                {
                    _streamDeckController.SetImage(keyIndex, ms);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UpdateImageFromBytes ({keyIndex}) failed");
            }
        }

        private async Task UpdateImageForKeyAsync(int keyIndex)
        {
            // On Cold Boot, web service might not be up yet, so let's retry a few times
            // to make sure everything is ready for us to call the web site.
            const int maxRetries = 3;
            var attempt = 0;

            while (attempt < maxRetries)
            {
                attempt++;
                try
                {
                    var imageBytes = await _streamDeckApi.GetImageForKeyAsync(keyIndex).ConfigureAwait(false);
                    _keyImageCache[keyIndex] = imageBytes;
                    UpdateImageFromBytes(keyIndex, imageBytes);

                    // Also update the keypressed image...
                    imageBytes = await _streamDeckApi.GetImageForKeyAsync(keyIndex + _streamDeckController.NumKeys).ConfigureAwait(false);
                    _keyImageCache[keyIndex + _streamDeckController.NumKeys] = imageBytes;
                }
                catch (Exception ex)
                {
                    if (attempt < maxRetries)
                    {
                        _logger.LogInformation($"SetImage ({keyIndex}) failed on attempt {attempt}.  Retrying.");
                        await Task.Delay(2000).ConfigureAwait(false);
                    }
                    else
                    {
                        _logger.LogError(ex, $"SetImage ({keyIndex}) failed");
                    }
                }
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

            await Task.WhenAll(tasks.ToArray()).ConfigureAwait(false);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _streamDeckController.FillAllKeysWithColor(0, 255, 0);
            await RefreshImagesAsync().ConfigureAwait(false);
        }
    }
}
