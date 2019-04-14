using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HomeControl.Web.Devices;
using HomeControl.Web.Services;

namespace HomeControl.Web.Activities
{
    public class TheaterPs4Activity : IActivity
    {
        private readonly ITheater _theater;

        public TheaterPs4Activity(ITheater theater)
        {
            _theater = theater;
        }

        public string Key => ActivityKey.TheaterOn;

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await _theater.Projector.TurnOnAsync().ConfigureAwait(false);
            await _theater.Receiver.TurnOnAsync().ConfigureAwait(false);
            await _theater.Receiver.SelectPs4InputAsync().ConfigureAwait(false);
        }
    }
}
