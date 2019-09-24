using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HomeControl.Web.Services
{
    public interface IStreamDeckActivityService
    {
        StreamDeckKeyInfo GetKeyInfoAtIndex(int keyIndex);
        Task ExecuteActivityAtIndexAsync(int keyIndex, CancellationToken cancellationToken);
    }
}
