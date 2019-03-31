using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HomeControl.Web.Services
{
    public interface IActivityService
    {
        Task ExecuteActivityAsync(string activityKey, CancellationToken cancellationToken);
    }
}
