using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using Volo.Abp.DependencyInjection;
using Warden.Messaging.Messages;

namespace Warden.ViewModels;

[Dependency(ServiceLifetime.Singleton)]
public sealed partial class MainWindowViewModel
    : ViewModel,
        IRecipient<ShowPageMessage>,
        IRecipient<SplashFinishedMessage>
{
    private bool _isInitialized;

    public required ISukiToastManager SukiToastManager { get; init; }
    public required ISukiDialogManager SukiDialogManager { get; init; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSplashView))]
    [NotifyCanExecuteChangedFor(nameof(ShowPageCommand))]
    public partial ViewModel ContentViewModel { get; set; } = null!;

    public bool IsSplashView => ContentViewModel is SplashViewModel;

    protected override bool CanExecuteShowPage() => !IsSplashView;

    public override void OnLoaded()
    {
        if (!_isInitialized)
        {
            ContentViewModel = ServiceProvider.GetRequiredService<SplashViewModel>();
        }

        _isInitialized = true;
    }

    public void Receive(ShowPageMessage message) => ChangePage(message);

    public void Receive(SplashFinishedMessage message) => ChangePage(message.ViewModelType);

    private void ChangePage(Type viewModelType)
    {
        ContentViewModel = (ViewModel)ServiceProvider.GetRequiredService(viewModelType);
    }
}
