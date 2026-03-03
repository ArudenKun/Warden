using CommunityToolkit.Mvvm.ComponentModel;
using Humanizer;
using Serilog.Events;

namespace Warden.Options;

public sealed partial class LoggingOptions : ObservableObject
{
    public const string Template =
        "[{Timestamp:yyyy-MM-dd HH:mm:ss}][{Level:u3}][{SourceContext}] {Message:lj}{NewLine}{Exception}";

    [ObservableProperty]
    public partial LogEventLevel LogEventLevel { get; set; } = LogEventLevel.Information;

    [ObservableProperty]
    public partial TimeSpan RetainedFileTimeLimit { get; set; } = 30.Days();
}
