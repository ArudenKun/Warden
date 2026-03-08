using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;
using Warden.Core.Navigation;
using Warden.Messaging.Messages;
using Warden.Views;

namespace Warden.ViewModels;

[Dependency(ServiceLifetime.Singleton)]
public sealed partial class SplashViewModel : ViewModel, INavigationAware
{
    [ObservableProperty]
    public partial string StatusText { get; set; } = "Initializing";

    public override void OnLoaded()
    {
        StartAsync().SafeFireAndForget();
    }

    private async Task StartAsync()
    {
        if (GeneralOptions.ShowConsole)
        {
            // Messenger.Send(new ConsoleWindowShowMessage());
        }

        await Task.Delay(1.Seconds());
        StatusText = "Loading Settings";
        await Task.Delay(200.Milliseconds());
        var message = GeneralOptions.IsSetup
            ? new SplashFinishedMessage(typeof(SetupView))
            : new SplashFinishedMessage(typeof(MainView));
        Messenger.Send(message);
    }

    public bool CanNavigateTo(object? parameter)
    {
        return true;
    }

    public void OnNavigatedTo(object? parameter) { }

    public bool CanNavigateFrom()
    {
        return true;
    }

    public void OnNavigatedFrom() { }
}
