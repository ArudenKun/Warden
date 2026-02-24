using Avalonia.Collections;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;
using ZLinq;

namespace Warden.ViewModels;

[Dependency(ServiceLifetime.Singleton)]
public sealed class SettingsViewModel : ViewModel
{
    public string DisplayName => "Settings";

    public IAvaloniaReadOnlyList<string> ColorThemes =>
        new AvaloniaList<string>(
            ThemeService.ColorThemes.AsValueEnumerable().Select(x => x.DisplayName).ToList()
        );
}
