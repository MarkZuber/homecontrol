using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HomeControl.Web.Devices;
using HomeControl.Web.Services;

namespace HomeControl.Web.Activities
{
    public class TheaterVolumeUpActivity : IActivity
    {
        private readonly IDenonNetworkReceiver _theaterReceiver;

        public TheaterVolumeUpActivity(IDenonNetworkReceiver theaterReceiver)
        {
            _theaterReceiver = theaterReceiver;
        }

        public string Key => ActivityKey.TheaterOn;

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await _theaterReceiver.TurnOffAsync().ConfigureAwait(false);

            // todo: turn off projector
        }
    }
}
