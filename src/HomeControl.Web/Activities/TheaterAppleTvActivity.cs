using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HomeControl.Web.Devices;
using HomeControl.Web.Services;

namespace HomeControl.Web.Activities
{
    public class TheaterAppleTvActivity : IActivity
    {
        private readonly IDenonNetworkReceiver _theaterReceiver;

        public TheaterAppleTvActivity(IDenonNetworkReceiver theaterReceiver)
        {
            _theaterReceiver = theaterReceiver;
        }

        public string Key => ActivityKey.TheaterOn;

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await _theaterReceiver.TurnOnAsync().ConfigureAwait(false);
            await _theaterReceiver.SelectAppleTvInputAsync().ConfigureAwait(false);
        }
    }
}
