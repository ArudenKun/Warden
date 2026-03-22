using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using R3;
using R3.ObservableEvents;
using Volo.Abp.DependencyInjection;
using Warden.Core;
using Warden.Core.FastUndoRedo;
using Warden.Core.Navigation;
using Warden.Views;
using ZLinq;

namespace Warden.ViewModels;

[Dependency(ServiceLifetime.Singleton)]
public sealed partial class SettingsViewModel : ViewModel, INavigationAware
{
    private Type? _callerViewType;

    public SettingsViewModel(ILoggerFactory loggerFactory)
    {
        UndoRedoService = new UndoRedoService(loggerFactory.CreateLogger("UndoRedo"));
    }

    public string DisplayName => "Settings";

    private UndoRedoService UndoRedoService { get; }

    public IAvaloniaReadOnlyList<string> ColorThemes =>
        new AvaloniaList<string>(
            ThemeService.ColorThemes.AsValueEnumerable().Select(x => x.DisplayName).ToList()
        );

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RedoCommand))]
    [FastUndoIgnore]
    public partial bool CanRedo { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(UndoCommand))]
    [FastUndoIgnore]
    public partial bool CanUndo { get; set; }

    [RelayCommand]
    private void Back()
    {
        NavigationHostManager.Navigate(Regions.Main, _callerViewType ?? typeof(MainView));
    }

    private bool CanExecuteRedo() => UndoRedoService.CanRedo;

    [RelayCommand(CanExecute = nameof(CanExecuteRedo))]
    private void Redo()
    {
        UndoRedoService.Redo();
    }

    private bool CanExecuteUndo() => UndoRedoService.CanUndo;

    [RelayCommand(CanExecute = nameof(CanExecuteUndo))]
    private void Undo()
    {
        UndoRedoService.Undo();
    }

    public override void OnLoaded()
    {
        base.OnLoaded();

        // AppearanceOptions.Events().PropertyChanged.Subscribe(_ => Notify()).AddTo(this);
        // GeneralOptions.Events().PropertyChanged.Subscribe(_ => Notify()).AddTo(this);
        // LoggingOptions.Events().PropertyChanged.Subscribe(_ => Notify()).AddTo(this);
        UndoRedoService
            .Events()
            .StateChanged.Subscribe(state =>
            {
                CanRedo = state.CanRedo;
                CanUndo = state.CanUndo;
            })
            .AddTo(this);

        UndoRedoService.Tracker.Register(this);
        UndoRedoService.Attach(AppearanceOptions);
        UndoRedoService.Attach(GeneralOptions);
        UndoRedoService.Attach(LoggingOptions);

        AppearanceOptions.AutoUpdate(LazyServiceProvider).AddTo(this);
        GeneralOptions.AutoUpdate(LazyServiceProvider).AddTo(this);
        LoggingOptions.AutoUpdate(LazyServiceProvider).AddTo(this);

        // UndoRedoService
        //     .Events()
        //     .PropertyChanged.Subscribe(x =>
        //     {
        //         RedoCommand.NotifyCanExecuteChanged();
        //         UndoCommand.NotifyCanExecuteChanged();
        //     })
        //     .AddTo(this);
        // AppearanceOptions.AsUndo(UndoRedoService).AddTo(this);
        // GeneralOptions.AsUndo(UndoRedoService).AddTo(this);
        // LoggingOptions.AsUndo(UndoRedoService).AddTo(this);
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
        UndoRedoService.Clear();
    }
}
