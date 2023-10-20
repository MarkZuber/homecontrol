using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using HomeControl.StreamDeck.Api;
using HomeControl.StreamDeck.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                    string logFilePrefix = "homecontrol.streamdeck";
                    string logsDir = "/tmp";
                    var logger = LoggerFactory.CreateLogger(logsDir, logFilePrefix);
                    services.AddSingleton(logger);

                    services.AddOptions();
                    services.Configure<StreamDeckConfig>(hostContext.Configuration.GetSection("StreamDeck"));

                    services.AddSingleton<IHostedService, StreamDeckService>();
                    services.AddSingleton<IStreamDeckApi>(new StreamDeckApi(logger, "http://192.168.2.203:8080"));
                });

            await builder.RunConsoleAsync().ConfigureAwait(false);
        }
    }
}
