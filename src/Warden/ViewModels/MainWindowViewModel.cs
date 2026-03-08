using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using Volo.Abp.DependencyInjection;
using Warden.Messaging.Messages;
using Warden.Views;

namespace Warden.ViewModels;

[Dependency(ServiceLifetime.Singleton)]
public sealed partial class MainWindowViewModel
    : ViewModel,
        // IRecipient<ShowPageMessage>,
        IRecipient<SplashFinishedMessage>
{
    private bool _isInitialized;

    public required ISukiToastManager SukiToastManager { get; init; }
    public required ISukiDialogManager SukiDialogManager { get; init; }

    public bool IsSplashView =>
        NavigationHostManager.GetHost(Regions.Main)?.CurrentContent is SplashView;

    public override void OnLoaded()
    {
        if (!_isInitialized)
        {
            OnPropertyChanged(nameof(IsSplashView));
            NavigationHostManager.Navigate<SplashView>(Regions.Main);
            OnPropertyChanged(nameof(IsSplashView));
            // ContentViewModel = ServiceProvider.GetRequiredService<SplashViewModel>();
        }

        _isInitialized = true;
    }

    [RelayCommand]
    private void ShowSettings()
    {
        var contentType = NavigationHostManager.GetHost(Regions.Main)?.CurrentContent?.GetType();
        NavigationHostManager.Navigate<SettingsView>(Regions.Main, contentType);
    }

    public void Receive(SplashFinishedMessage message)
    {
        NavigationHostManager.Navigate(Regions.Main, message.ViewType);
        OnPropertyChanged(nameof(IsSplashView));
    }
}
