using Serilog.Events;
using Velopack.Logging;

namespace Warden.Utilities;

public sealed class VelopackLogger : IVelopackLogger
{
    public void Log(VelopackLogLevel logLevel, string? message, Exception? exception)
    {
        LogHelper.Log(nameof(Velopack), MapLogLevel(logLevel), exception, message ?? string.Empty);
    }

    private static LogEventLevel MapLogLevel(VelopackLogLevel level) =>
        level switch
        {
            VelopackLogLevel.Trace => LogEventLevel.Verbose,
            VelopackLogLevel.Debug => LogEventLevel.Debug,
            VelopackLogLevel.Information => LogEventLevel.Information,
            VelopackLogLevel.Warning => LogEventLevel.Warning,
            VelopackLogLevel.Error => LogEventLevel.Error,
            VelopackLogLevel.Critical => LogEventLevel.Fatal,
            _ => throw new InvalidOperationException("Invalid log level"),
        };
}
