using Microsoft.Extensions.Logging;
using Serilog.Events;

namespace Warden.Utilities.Extensions;

public static class SerilogExtensions
{
    public static LogEventLevel ToLogEventLevel(this LogLevel level) =>
        level switch
        {
            LogLevel.Trace => LogEventLevel.Verbose,
            LogLevel.Debug => LogEventLevel.Debug,
            LogLevel.Information => LogEventLevel.Information,
            LogLevel.Warning => LogEventLevel.Warning,
            LogLevel.Error => LogEventLevel.Error,
            LogLevel.Critical => LogEventLevel.Fatal,
            _ => LogEventLevel.Information,
        };
}
