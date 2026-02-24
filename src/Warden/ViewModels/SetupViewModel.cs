using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Warden.Messaging.Messages;

namespace Warden.ViewModels;

public sealed partial class SetupViewModel : ViewModel, IRecipient<NextSetupMessage>
{
    [ObservableProperty]
    public partial ViewModel ViewModel { get; set; } = null!;

    [ObservableProperty]
    public partial int Limit { get; set; } = 5;

    [RelayCommand]
    private void Finish()
    {
        GeneralSetting.IsSetup = false;
    }

    public override void OnLoaded()
    {
        ViewModel = ServiceProvider.GetRequiredService<SetupThemeViewModel>();
    }

    public void Receive(NextSetupMessage message)
    {
        ViewModel = (ViewModel)ServiceProvider.GetRequiredService(message.ViewModelType);
    }
}
