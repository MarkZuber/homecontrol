using System.Collections.Concurrent;

namespace HomeControl.Web.Services
{
    public class AlexaActivityServiceConfig
    {
        public AlexaActivityServiceConfig(ConcurrentDictionary<string, string> alexaToActivityKeys)
        {
            AlexaToActivityKeys = alexaToActivityKeys;
        }

        public ConcurrentDictionary<string, string> AlexaToActivityKeys { get; }
    }
}
