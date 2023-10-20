using System.IO;
using Serilog;
using Serilog.Formatting.Compact;

namespace HomeControl.StreamDeck;

public static class LoggerFactory
{
    public static ILogger CreateLogger(string logsDir, string logFilePrefix)
    {
        var config = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .WriteTo.File(path: Path.Combine(logsDir, $"{logFilePrefix}_.log"),
                formatter: new RenderedCompactJsonFormatter(),
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true,
                retainedFileCountLimit: 10);

        return config.CreateLogger();
    }
}