using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;
using Warden.Core.Navigation;
using Warden.Core.Options;
using Warden.Options;
using Warden.Views;
using ZLinq;

namespace Warden.ViewModels;

[Dependency(ServiceLifetime.Singleton)]
public sealed partial class SettingsViewModel : ViewModel, INavigationAware
{
    private readonly IOptionsMutable<LoggingOptions> _loggingOptions;
    private Type? _callerViewType;

    public SettingsViewModel(IOptionsMutable<LoggingOptions> loggingOptions)
    {
        _loggingOptions = loggingOptions;
    }

    public string DisplayName => "Settings";

    public IAvaloniaReadOnlyList<string> ColorThemes =>
        new AvaloniaList<string>(
            ThemeService.ColorThemes.AsValueEnumerable().Select(x => x.DisplayName).ToList()
        );

    [RelayCommand]
    private void Back()
    {
        NavigationHostManager.Navigate(Regions.Main, _callerViewType ?? typeof(MainView));
    }

    public bool CanNavigateTo(object? parameter)
    {
        return true;
    }

    public void OnNavigatedTo(object? parameter)
    {
        if (parameter is Type type)
        {
            _callerViewType = type;
        }
    }

    public bool CanNavigateFrom()
    {
        return true;
    }

    public void OnNavigatedFrom() { }
}
