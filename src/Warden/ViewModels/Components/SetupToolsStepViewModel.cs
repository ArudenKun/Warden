using CommunityToolkit.Mvvm.Messaging;
using Warden.Core.Navigation;
using Warden.Messaging.Messages;

namespace Warden.ViewModels.Components;

public sealed partial class SetupToolsStepViewModel : SetupStepViewModel, INavigationAware
{
    public override int StepIndex => 1;
    public override string Title => "Tools";

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
}
