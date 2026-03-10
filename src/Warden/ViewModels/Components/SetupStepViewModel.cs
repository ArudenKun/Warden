using SukiUI.Controls;

namespace Warden.ViewModels.Components;

public abstract partial class SetupStepViewModel : ViewModel, ISukiStackPageTitleProvider
{
    public abstract int StepIndex { get; }

    public abstract string Title { get; }
}
