using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using R3;
using R3.ObservableEvents;
using Volo.Abp.DependencyInjection;
using Warden.Core;
using Warden.Core.Histories.Extensions;
using Warden.Core.Navigation;
using Warden.Views;
using ZLinq;

namespace Warden.ViewModels;

[Dependency(ServiceLifetime.Singleton)]
public sealed partial class SettingsViewModel : ViewModel, INavigationAware
{
    private Type? _callerViewType;

    public string DisplayName => "Settings";

    public IAvaloniaReadOnlyList<string> ColorThemes =>
        new AvaloniaList<string>(
            ThemeService.ColorThemes.AsValueEnumerable().Select(x => x.DisplayName).ToList()
        );

    [RelayCommand]
    private void Back()
    {
        NavigationHostManager.Navigate(Regions.Main, _callerViewType ?? typeof(MainView));
    }

    private bool CanExecuteRedo() => UndoManager.CanRedo;

    [RelayCommand(CanExecute = nameof(CanExecuteRedo))]
    private void Redo()
    {
        UndoManager.Redo();
        UndoCommand.NotifyCanExecuteChanged();
    }

    private bool CanExecuteUndo() => UndoManager.CanUndo;

    [RelayCommand(CanExecute = nameof(CanExecuteUndo))]
    private void Undo()
    {
        UndoManager.Undo();
        RedoCommand.NotifyCanExecuteChanged();
    }

    public override void OnLoaded()
    {
        base.OnLoaded();

        UndoManager
            .Events()
            .PropertyChanged.Subscribe(x =>
            {
                RedoCommand.NotifyCanExecuteChanged();
                UndoCommand.NotifyCanExecuteChanged();
            })
            .AddTo(this);
        AppearanceOptions.AsUndo(UndoManager).AddTo(this);
        GeneralOptions.AsUndo(UndoManager).AddTo(this);
        LoggingOptions.AsUndo(UndoManager).AddTo(this);
    }

    public bool CanNavigateTo(object? parameter)
    {
        return true;
    }

    public void OnNavigatedTo(object? parameter)
    {
        if (parameter is Type type)
        {
            _callerViewType = type;
        }
    }

    public bool CanNavigateFrom() => true;

    public void OnNavigatedFrom()
    {
        Logger.LogInformation("OnNavigatedFrom");
        UndoManager.Clear();
    }
}
