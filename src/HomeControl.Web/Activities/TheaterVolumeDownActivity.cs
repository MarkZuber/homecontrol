﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HomeControl.Web.Devices;
using HomeControl.Web.Services;

namespace HomeControl.Web.Activities
{
    public class TheaterVolumeDownActivity : IActivity
    {
        private readonly ITheater _theater;

        public TheaterVolumeDownActivity(ITheater theater)
        {
            _theater = theater;
        }

        public string Key => ActivityKey.TheaterOn;

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await _theater.Receiver.VolumeDownAsync().ConfigureAwait(false);
        }
    }
}
