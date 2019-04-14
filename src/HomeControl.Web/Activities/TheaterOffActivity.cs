using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HomeControl.Web.Devices;
using HomeControl.Web.Services;

namespace HomeControl.Web.Activities
{
    public class TheaterOffActivity : IActivity
    {
        private readonly ITheater _theater;

        public TheaterOffActivity(ITheater theater)
        {
            _theater = theater;
        }

        public string Key => ActivityKey.TheaterOn;

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await _theater.Projector.TurnOffAsync().ConfigureAwait(false);
            await _theater.Receiver.TurnOffAsync().ConfigureAwait(false);
        }
    }
}
