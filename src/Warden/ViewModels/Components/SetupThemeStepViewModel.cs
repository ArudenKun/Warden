using Warden.Core.Navigation;

namespace Warden.ViewModels.Components;

public partial class SetupThemeStepViewModel : SetupStepViewModel, INavigationAware
{
    public override int StepIndex => 0;
    public override string Title => "Theme";

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
