using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Warden.Core.Navigation;
using Warden.Messaging.Messages;
using Warden.ViewModels.Components;
using Warden.Views;
using ZLinq;

namespace Warden.ViewModels;

public sealed partial class SetupViewModel
    : ViewModel,
        INavigationAware,
        IRecipient<SetupFinishedMessage>
{
    private readonly Queue<SetupStepViewModel> _nextSteps = new();
    private readonly Stack<SetupStepViewModel> _backSteps = new();

    private bool _isInitialized;

    public SetupViewModel(IEnumerable<SetupStepViewModel> stepViewModels)
    {
        foreach (var stepViewModel in stepViewModels.AsValueEnumerable().OrderBy(x => x.StepIndex))
        {
            _nextSteps.Enqueue(stepViewModel);
        }
    }

    [ObservableProperty]
    public partial string Header { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int StepIndex { get; set; } = 0;

    [ObservableProperty]
    public partial SetupStepViewModel? Step { get; set; }

    [RelayCommand]
    private void Next()
    {
        if (_nextSteps.Count is 0)
        {
            Navigate<MainView>();
            return;
        }

        Step = _nextSteps.Dequeue();
        Header = Step.Header;
        StepIndex = Step.StepIndex;
        _backSteps.Push(Step);
        BackCommand.NotifyCanExecuteChanged();
    }

    private bool CanExecuteBack() => _backSteps.Count > 0;

    [RelayCommand(CanExecute = nameof(CanExecuteBack))]
    private void Back()
    {
        Step = _backSteps.Pop();
        Header = Step.Header;
        StepIndex = Step.StepIndex;
        _nextSteps.Enqueue(Step);
        BackCommand.NotifyCanExecuteChanged();
    }

    public bool CanNavigateTo(object? parameter)
    {
        return true;
    }

    public void OnNavigatedTo(object? parameter)
    {
        Next();
    }

    public bool CanNavigateFrom()
    {
        return true;
    }

    public void OnNavigatedFrom()
    {
        foreach (var stepViewModel in _nextSteps)
        {
            stepViewModel.Dispose();
        }

        foreach (var stepViewModel in _backSteps)
        {
            stepViewModel.Dispose();
        }
    }

    public override void OnLoaded()
    {
        BackCommand.NotifyCanExecuteChanged();
    }

    public void Receive(SetupFinishedMessage message) => Navigate<MainView>();
}
