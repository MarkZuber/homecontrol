﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HomeControl.Web.Devices;
using HomeControl.Web.Services;
using Serilog;

namespace HomeControl.Web.Activities
{
    public class TheaterAppleTvActivity : IActivity
    {
        private readonly ILogger _logger;
        private readonly ITheater _theater;

        public TheaterAppleTvActivity(ILogger logger, ITheater theater)
        {
            _logger = logger;
            _theater = theater;
        }

        public string Key => ActivityKey.TheaterOn;

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.Information("AppleTV Activity");
            try
            {
                await _theater.EpsonProjector.TurnOnAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Error($"XboxActivity Failure to turn on Projector: {ex.Message}");
            }

            try
            {
                await _theater.Receiver.TurnOnAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Error($"XboxActivity Failure to turn on Receiver: {ex.Message}");
            }

            try
            {
                await _theater.Receiver.SelectAppleTvInputAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Error($"XboxActivity Failure to set input on Receiver: {ex.Message}");
            }
        }
    }
}
