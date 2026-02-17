using R3;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Warden.Settings;
using Warden.Utilities.Extensions;

namespace Warden.Utilities;

public static class LogHelper
{
    private static bool _isDisposed;
    private static bool _isInitialized;
    private static IDisposable? _subscription;

    // private static readonly List<(
    //     string? Context,
    //     Type? ContextType,
    //     LogEventLevel Level,
    //     Exception? Exception,
    //     string MessageTemplate,
    //     object?[]? Args
    // )> Logs = [];

    public static LoggingLevelSwitch? LoggingLevelSwitch { get; set; }

    private static ILogger Logger => Serilog.Log.Logger;

    public static void Initialize(LoggingSetting loggingSetting)
    {
        if (_isInitialized)
            return;

        _subscription = loggingSetting
            .ObservePropertyChanged(x => x.LogEventLevel)
            .Subscribe(x => LoggingLevelSwitch?.MinimumLevel = x);

        // Flush();
        _isInitialized = true;
    }

    public static void Cleanup()
    {
        if (_isDisposed)
            return;

        // Logger = null;
        _subscription?.Dispose();
        _isDisposed = true;
    }

    // private static void Flush()
    // {
    //     if (Logs.Count == 0)
    //         return;
    //
    //     foreach (var (context, contextType, level, exception, template, args) in Logs)
    //     {
    //         WriteInternal(context, contextType, level, exception, template, args);
    //     }
    //
    //     Logs.Clear();
    // }

    #region Core Logging

    private static void WriteInternal(
        string? context,
        Type? contextType,
        LogEventLevel level,
        Exception? exception,
        string messageTemplate,
        object?[]? args
    )
    {
        // if (Logger is null)
        // {
        //     Logs.Add((context, contextType, level, exception, messageTemplate, args));
        //     return;
        // }

        var logger = Logger;

        if (!string.IsNullOrWhiteSpace(context))
            logger = logger.ForContext("SourceContext", context);
        else if (contextType is not null)
            logger = logger.ForContext(contextType);
        else
            logger = logger.ForContext("SourceContext", "Global");

        logger.Write(level, exception, messageTemplate, args);
    }

    #endregion

    #region Public Log Methods

    // --- Base ---
    public static void Log(
        LogEventLevel level,
        Exception? exception,
        string messageTemplate,
        params object?[]? args
    ) => WriteInternal(null, null, level, exception, messageTemplate, args);

    // --- String context ---
    public static void Log(
        string context,
        LogEventLevel level,
        Exception? exception,
        string messageTemplate,
        params object?[]? args
    ) => WriteInternal(context, null, level, exception, messageTemplate, args);

    // --- Generic context ---
    public static void Log<TContext>(
        LogEventLevel level,
        Exception? exception,
        string messageTemplate,
        params object?[]? args
    ) => WriteInternal(null, typeof(TContext), level, exception, messageTemplate, args);

    #endregion

    #region Level-specific Shortcuts

    public static void Verbose(string messageTemplate, params object?[]? args) =>
        Log(LogEventLevel.Verbose, null, messageTemplate, args);

    public static void Verbose(string context, string messageTemplate, params object?[]? args) =>
        Log(context, LogEventLevel.Verbose, null, messageTemplate, args);

    public static void Verbose<TContext>(string messageTemplate, params object?[]? args) =>
        Log<TContext>(LogEventLevel.Verbose, null, messageTemplate, args);

    public static void Debug(string messageTemplate, params object?[]? args) =>
        Log(LogEventLevel.Debug, null, messageTemplate, args);

    public static void Debug(string context, string messageTemplate, params object?[]? args) =>
        Log(context, LogEventLevel.Debug, null, messageTemplate, args);

    public static void Debug<TContext>(string messageTemplate, params object?[]? args) =>
        Log<TContext>(LogEventLevel.Debug, null, messageTemplate, args);

    public static void Information(string messageTemplate, params object?[]? args) =>
        Log(LogEventLevel.Information, null, messageTemplate, args);

    public static void Information(
        string context,
        string messageTemplate,
        params object?[]? args
    ) => Log(context, LogEventLevel.Information, null, messageTemplate, args);

    public static void Information<TContext>(string messageTemplate, params object?[]? args) =>
        Log<TContext>(LogEventLevel.Information, null, messageTemplate, args);

    public static void Warning(string messageTemplate, params object?[]? args) =>
        Log(LogEventLevel.Warning, null, messageTemplate, args);

    public static void Warning(string context, string messageTemplate, params object?[]? args) =>
        Log(context, LogEventLevel.Warning, null, messageTemplate, args);

    public static void Warning<TContext>(string messageTemplate, params object?[]? args) =>
        Log<TContext>(LogEventLevel.Warning, null, messageTemplate, args);

    public static void Error(Exception exception, string messageTemplate, params object?[]? args) =>
        Log(LogEventLevel.Error, exception, messageTemplate, args);

    public static void Error(
        string context,
        Exception exception,
        string messageTemplate,
        params object?[]? args
    ) => Log(context, LogEventLevel.Error, exception, messageTemplate, args);

    public static void Error<TContext>(
        Exception exception,
        string messageTemplate,
        params object?[]? args
    ) => Log<TContext>(LogEventLevel.Error, exception, messageTemplate, args);

    public static void Fatal(Exception exception, string messageTemplate, params object?[]? args) =>
        Log(LogEventLevel.Fatal, exception, messageTemplate, args);

    public static void Fatal(
        string context,
        Exception exception,
        string messageTemplate,
        params object?[]? args
    ) => Log(context, LogEventLevel.Fatal, exception, messageTemplate, args);

    public static void Fatal<TContext>(
        Exception exception,
        string messageTemplate,
        params object?[]? args
    ) => Log<TContext>(LogEventLevel.Fatal, exception, messageTemplate, args);

    #endregion
}
