using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using HomeControl.StreamDeck.Api;
using HomeControl.StreamDeck.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HomeControl.StreamDeck
{
    public static class Program
    {
        [SuppressMessage("AsyncUsage.CSharp.Naming", "UseAsyncSuffix", Justification = "Override of uncontrolled API.")]
        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                    services.Configure<StreamDeckConfig>(hostContext.Configuration.GetSection("StreamDeck"));

                    services.AddSingleton<IHostedService, StreamDeckService>();
                    services.AddSingleton<IStreamDeckApi>(new StreamDeckApi("http://192.168.2.203:8080"));
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                });

            await builder.RunConsoleAsync().ConfigureAwait(false);
        }
    }
}
