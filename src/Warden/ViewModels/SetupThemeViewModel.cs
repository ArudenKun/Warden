using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SukiUI.Controls;
using Warden.Messaging.Messages;

namespace Warden.ViewModels;

public sealed partial class SetupThemeViewModel : ViewModel, ISukiStackPageTitleProvider
{
    string ISukiStackPageTitleProvider.Title => "Theme";

    [RelayCommand]
    private void Finish()
    {
        Messenger.Send(new NextSetupMessage(typeof(SetupToolsViewModel)));
    }
}
