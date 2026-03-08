namespace Warden.ViewModels.Components;

public abstract partial class SetupStepViewModel : ViewModel
{
    public abstract int StepIndex { get; }

    public abstract string Header { get; }
}
