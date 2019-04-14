using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace HomeControl.Web.Devices.Transports
{
    public class HttpTransport : IHttpTransport
    {
        public string IpAddress { get; set; }
        public int Port { get; set; } = 80;

        public async Task<string> ExecuteGetAsync(string uriPath)
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri($"http://{IpAddress}:{Port}")
            };
            return await httpClient.GetStringAsync(uriPath);
        }
    }
}
