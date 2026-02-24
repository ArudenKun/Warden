using CommunityToolkit.Mvvm.Input;

namespace Warden.ViewModels;

public sealed partial class SetupToolsViewModel : ViewModel
{
    [RelayCommand]
    private async Task FinishAsync()
    {
        await ShowPageAsync<MainViewModel>();
    }
}
