using Avalonia.Collections;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;
using Warden.Core.Options;
using Warden.Options;
using ZLinq;

namespace Warden.ViewModels;

[Dependency(ServiceLifetime.Singleton)]
public sealed class SettingsViewModel : ViewModel
{
    private readonly IOptionsMutable<LoggingOptions> _loggingOptions;

    public SettingsViewModel(IOptionsMutable<LoggingOptions> loggingOptions)
    {
        _loggingOptions = loggingOptions;
    }

    public string DisplayName => "Settings";

    public IAvaloniaReadOnlyList<string> ColorThemes =>
        new AvaloniaList<string>(
            ThemeService.ColorThemes.AsValueEnumerable().Select(x => x.DisplayName).ToList()
        );
}
