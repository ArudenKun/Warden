using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Warden.Core.Navigation;
using Warden.Messaging.Messages;
using Warden.ViewModels.Components;
using Warden.Views;

namespace Warden.ViewModels;

public sealed partial class SetupViewModel
    : ViewModel,
        INavigationAware,
        IRecipient<SetupFinishedMessage>
{
    private readonly List<SetupStepViewModel> _allSteps;
    private readonly Stack<SetupStepViewModel> _backStack = new();

    public SetupViewModel(IEnumerable<SetupStepViewModel> stepViewModels)
    {
        _allSteps = stepViewModels.OrderByDescending(x => x.StepIndex).ToList();
        Steps = new Queue<SetupStepViewModel>(_allSteps);
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(BackCommand))]
    [NotifyCanExecuteChangedFor(nameof(NextCommand))]
    public partial SetupStepViewModel? Step { get; set; }

    public Queue<SetupStepViewModel> Steps { get; private set; }

    private bool CanExecuteNext() => Steps.Count > 0;

    [RelayCommand(CanExecute = nameof(CanExecuteNext))]
    private void Next()
    {
        if (Step is not null)
        {
            _backStack.Push(Step);
        }

        // 2. Fix: Use TryDequeue to prevent exceptions if the queue is empty
        if (Steps.TryDequeue(out var nextStep))
        {
            Step = nextStep;
        }
        else
        {
            Navigate<MainView>();
        }

        BackCommand.NotifyCanExecuteChanged();
    }

    private bool CanExecuteBack() => _backStack.Count > 0;

    [RelayCommand(CanExecute = nameof(CanExecuteBack))]
    private void Back()
    {
        if (_backStack.TryPop(out var previousStep))
        {
            if (Step is not null)
            {
                var remainingSteps = Steps.ToList();
                remainingSteps.Insert(0, Step);
                Steps = new Queue<SetupStepViewModel>(remainingSteps);
            }

            Step = previousStep;
        }

        NextCommand.NotifyCanExecuteChanged();
    }

    public bool CanNavigateTo(object? parameter)
    {
        return true;
    }

    public void OnNavigatedTo(object? parameter)
    {
        ResetSteps();
        Next();
    }

    public bool CanNavigateFrom()
    {
        return true;
    }

    public void OnNavigatedFrom() { }

    private void ResetSteps()
    {
        _backStack.Clear();
        Steps = new Queue<SetupStepViewModel>(_allSteps);
        Step = null;
    }

    public void Receive(SetupFinishedMessage message) => Navigate<MainView>();
}
