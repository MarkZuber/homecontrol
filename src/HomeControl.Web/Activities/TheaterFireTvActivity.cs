using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HomeControl.Web.Devices;
using HomeControl.Web.Services;

namespace HomeControl.Web.Activities
{
    public class TheaterFireTvActivity : IActivity
    {
        private readonly ITheater _theater;

        public TheaterFireTvActivity(ITheater theater)
        {
            _theater = theater;
        }

        public string Key => ActivityKey.TheaterOn;

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await _theater.EpsonProjector.TurnOnAsync().ConfigureAwait(false);
            await _theater.Receiver.TurnOnAsync().ConfigureAwait(false);
            await _theater.Receiver.SelectFireTvInputAsync().ConfigureAwait(false);
        }
    }
}
