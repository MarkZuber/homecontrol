using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControl.Web.Services
{
    public class StreamDeckActivityServiceConfig
    {
        public StreamDeckActivityServiceConfig(ConcurrentDictionary<int, StreamDeckKeyInfo> keyInfos)
        {
            KeyInfos = keyInfos;
        }

        public ConcurrentDictionary<int, StreamDeckKeyInfo> KeyInfos { get; }
    }
}
