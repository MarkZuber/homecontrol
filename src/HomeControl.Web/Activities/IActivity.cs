using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HomeControl.Web.Activities
{
    public interface IActivity
    {
        string Key { get; }
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
