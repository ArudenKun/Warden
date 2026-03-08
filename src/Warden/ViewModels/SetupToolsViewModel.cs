using CommunityToolkit.Mvvm.Input;
using SukiUI.Controls;
using Warden.Views;

namespace Warden.ViewModels;

public sealed partial class SetupToolsViewModel : ViewModel, ISukiStackPageTitleProvider
{
    string ISukiStackPageTitleProvider.Title => "Tools";

    [RelayCommand]
    private async Task FinishAsync()
    {
        GeneralOptions.IsSetup = false;
        await ShowPageAsync<MainView>();
    }
}
