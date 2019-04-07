using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HomeControl.Web.Services;

namespace HomeControl.Web.Activities
{
    public class TheaterNullActivity : IActivity
    {
        public TheaterNullActivity()
        {
        }

        public string Key => ActivityKey.None;

        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
