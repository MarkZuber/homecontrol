using System.Threading.Tasks;

namespace HomeControl.Web.Devices.Transports
{
    public interface IHttpTransport
    {
        string IpAddress { get; set; }
        int Port { get; set; }

        Task<string> ExecuteGetAsync(string uriPath);
    }
}
