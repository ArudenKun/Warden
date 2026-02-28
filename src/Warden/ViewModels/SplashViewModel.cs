using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;
using Warden.Core;
using Warden.Messaging.Messages;

namespace Warden.ViewModels;

[Dependency(ServiceLifetime.Singleton)]
public sealed partial class SplashViewModel : ViewModel
{
    [ObservableProperty]
    public partial string StatusText { get; set; } = "Initializing";

    public override void OnLoaded()
    {
        StartAsync().SafeFireAndForget();
    }

    private async Task StartAsync()
    {
        if (GeneralSetting.ShowConsole)
        {
            // Messenger.Send(new ConsoleWindowShowMessage());
        }

        await Task.Delay(1.Seconds());
        StatusText = "Loading Settings";
        await Task.Delay(200.Milliseconds());
        var message = GeneralSetting.IsSetup
            ? new SplashFinishedMessage(typeof(SetupViewModel))
            : new SplashFinishedMessage(typeof(MainViewModel));
        Messenger.Send(message);
    }
}
