using System.IO;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Sinks.SystemConsole.Themes;

public static class LoggerFactory
{
    public static ILogger CreateLogger(string logsDir, string logFilePrefix)
    {
        var config = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .WriteTo.Console(
                outputTemplate: "{Timestamp:HH:mm:ss.fff} {Level:w3} {Message:lj}{NewLine}{Exception}",
                theme: AnsiConsoleTheme.Code)
            .WriteTo.File(path: Path.Combine(logsDir, $"{logFilePrefix}_.log"),
                formatter: new RenderedCompactJsonFormatter(),
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true,
                retainedFileCountLimit: 10);

        return config.CreateLogger();
    }
}