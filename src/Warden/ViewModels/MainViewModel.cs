using CommunityToolkit.Mvvm.Input;
using Warden.Core.Modrinth;

namespace Warden.ViewModels;

public sealed partial class MainViewModel : ViewModel
{
    private readonly IModrinthClient _modrinthClient;

    public MainViewModel(IModrinthClient modrinthClient)
    {
        _modrinthClient = modrinthClient;
    }

    [RelayCommand]
    private async Task Start()
    {
        _modrinthClient.Projects.SearchAsync("");
        _modrinthClient.Projects.GetDependenciesAsync("");
    }
}
