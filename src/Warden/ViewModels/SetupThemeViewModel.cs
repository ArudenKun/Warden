using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Warden.Messaging.Messages;

namespace Warden.ViewModels;

public sealed partial class SetupThemeViewModel : ViewModel
{
    [RelayCommand]
    private void Finish()
    {
        Messenger.Send(new NextSetupMessage(typeof(SetupToolsViewModel)));
    }
}
