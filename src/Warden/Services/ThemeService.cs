using Antelcat.AutoGen.ComponentModel.Diagnostic;
using Avalonia.Collections;
using Avalonia.Media;
using Avalonia.Styling;
using R3;
using SukiUI;
using SukiUI.Enums;
using SukiUI.Models;
using Volo.Abp.DependencyInjection;
using Warden.Models;
using Warden.Services.Settings;
using Warden.Settings;
using ZLinq;

namespace Warden.Services;

[AutoExtractInterface(Interfaces = [typeof(IDisposable)])]
public sealed class ThemeService : IThemeService, ISingletonDependency
{
    private readonly IDisposable _subscriptions;
    private readonly AppearanceSetting _appearanceSetting;

    private bool _initialized;

    public ThemeService(ISettingsService settingsService)
    {
        _appearanceSetting = settingsService.Get<AppearanceSetting>();
        _subscriptions = Disposable.Combine(
            _appearanceSetting
                .ObservePropertyChanged(x => x.Theme, false)
                .ObserveOnUIThreadDispatcher()
                .Subscribe(ChangeTheme),
            _appearanceSetting
                .ObservePropertyChanged(x => x.ThemeColor)
                .ObserveOnUIThreadDispatcher()
                .Subscribe(colorThemeDisplayName =>
                    ChangeColorTheme(ResolveColorTheme(colorThemeDisplayName))
                )
        );
    }

    private static SukiTheme SukiTheme => field ??= SukiTheme.GetInstance();

    public Theme CurrentTheme => _appearanceSetting.Theme;

    public SukiColorTheme CurrentColorTheme => ResolveColorTheme(_appearanceSetting.ThemeColor);

    public IAvaloniaReadOnlyList<SukiColorTheme> ColorThemes => SukiTheme.ColorThemes;

    public void Initialize()
    {
        if (_initialized)
            return;

        SukiTheme.AddColorThemes([
            new SukiColorTheme("Pink", new Color(255, 255, 20, 147), new Color(255, 255, 192, 203)),
            new SukiColorTheme("White", new Color(255, 255, 255, 255), new Color(255, 0, 0, 0)),
            new SukiColorTheme("Black", new Color(255, 0, 0, 0), new Color(255, 255, 255, 255)),
        ]);
        ChangeTheme(_appearanceSetting.Theme);
        ChangeColorTheme(ResolveColorTheme(_appearanceSetting.ThemeColor));
        _initialized = true;
    }

    public void ChangeTheme(Theme theme)
    {
        _appearanceSetting.Theme = theme;
        var variant = theme switch
        {
            Theme.System => ThemeVariant.Default,
            Theme.Light => ThemeVariant.Light,
            Theme.Dark => ThemeVariant.Dark,
            _ => throw new ArgumentOutOfRangeException(nameof(theme), theme, null),
        };
        SukiTheme.ChangeBaseTheme(variant);
    }

    public void ChangeColorTheme(SukiColorTheme colorTheme)
    {
        _appearanceSetting.ThemeColor = colorTheme.DisplayName;
        SukiTheme.ChangeColorTheme(colorTheme);
    }

    private static SukiColorTheme ResolveColorTheme(string? displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            return SukiTheme.DefaultColorThemes[SukiColor.Blue];

        return SukiTheme
                .ColorThemes.AsValueEnumerable()
                .FirstOrDefault(theme =>
                    theme.DisplayName.Equals(displayName, StringComparison.OrdinalIgnoreCase)
                )
            ?? SukiTheme.DefaultColorThemes[SukiColor.Blue];
    }

    public void Dispose() => _subscriptions.Dispose();
}
