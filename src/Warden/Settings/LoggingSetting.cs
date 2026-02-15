using CommunityToolkit.Mvvm.ComponentModel;
using Humanizer;
using Microsoft.Extensions.Logging;

namespace Warden.Settings;

public sealed partial class LoggingSetting : ObservableObject
{
    public const string Template =
        "[{Timestamp:yyyy-MM-dd HH:mm:ss}][{Level:u3}][{SourceContext}] {Message:lj}{NewLine}{Exception}";

    [ObservableProperty]
    public partial LogLevel LogLevel { get; set; } = LogLevel.Information;

    [ObservableProperty]
    public partial TimeSpan RetainedFileTimeLimit { get; set; } = 30.Days();
}
