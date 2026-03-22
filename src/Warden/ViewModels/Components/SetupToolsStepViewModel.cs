using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Humanizer;
using Microsoft.Extensions.Logging;
using R3;
using Warden.Core;
using Warden.Core.Modrinth;
using Warden.Core.Navigation;
using ZLinq;

namespace Warden.ViewModels.Components;

public sealed partial class SetupToolsStepViewModel : SetupStepViewModel, INavigationAware
{
    private readonly IModrinthClient _modrinthClient;

    public SetupToolsStepViewModel(IModrinthClient modrinthClient)
    {
        _modrinthClient = modrinthClient;

        Search = new BindableReactiveProperty<string>(string.Empty);
        Search.Debounce(150.Milliseconds()).Subscribe().AddTo(this);
    }

    public override int StepIndex => 1;
    public override string Title => "Tools";

    public BindableReactiveProperty<string> Search { get; }

    [ObservableProperty]
    public partial string Result { get; set; } = string.Empty;

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

    [RelayCommand]
    private async Task SubmitAsync(CancellationToken cancellationToken)
    {
        try
        {
            Result = string.Empty;
            var response = await _modrinthClient.Projects.SearchAsync(
                Search.Value,
                Facet.Create().ProjectType(ProjectType.Mod).Build(),
                cancellationToken: cancellationToken
            );

            if (!response.IsSuccessful)
            {
                Result = response.Error.Message;
                return;
            }

            var hits = response.Content.Hits;
            if (hits.Count is 0)
            {
                Result = "No results found";
                return;
            }

            var ids = hits.AsValueEnumerable()
                .Select(projectResult => projectResult.ProjectId)
                .ToArray();
            var response2 = await _modrinthClient.Projects.GetAsync(ids, cancellationToken);
            if (!response2.IsSuccessful)
            {
                Result = response2.Error.Message;
                return;
            }
            Logger.LogDebug(response2.RequestMessage?.RequestUri?.ToString());
            var projects = response2.Content ?? [];
            Result = JsonSerializer.Serialize(projects);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
        }
    }
}
