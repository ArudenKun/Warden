using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using Volo.Abp.DependencyInjection;
using Warden.Messaging.Messages;
using Warden.ViewModels.Components;

namespace Warden.ViewModels;

[Dependency(ServiceLifetime.Singleton)]
public sealed partial class MainWindowViewModel : ViewModel, IRecipient<SplashViewFinishedMessage>
{
    public required ISukiToastManager SukiToastManager { get; init; }
    public required ISukiDialogManager SukiDialogManager { get; init; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsMainView))]
    [NotifyCanExecuteChangedFor(nameof(ShowPageCommand))]
    public partial ViewModel ContentViewModel { get; set; } = null!;

    public bool IsMainView => ContentViewModel is MainViewModel;

    public override void OnLoaded()
    {
        ContentViewModel = ServiceProvider.GetRequiredService<SplashViewModel>();
    }

    [RelayCommand(CanExecute = nameof(IsMainView))]
    private Task ShowPageAsync(Type pageType)
    {
        Messenger.Send(new ShowPageMessage(pageType));
        return Task.CompletedTask;
    }

    public void Receive(SplashViewFinishedMessage message)
    {
        ContentViewModel = ServiceProvider.GetRequiredService<MainViewModel>();
    }
}
