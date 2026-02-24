using System.Diagnostics;
using Avalonia.Input.Platform;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using R3;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;
using Warden.Messaging.Messages;
using Warden.Services;
using Warden.Services.Settings;
using Warden.Settings;

namespace Warden.ViewModels;

[PublicAPI]
public abstract partial class ViewModel
    : ObservableValidator,
        IDisposable,
        ITransientDependency,
        IHasExtraProperties
{
    private bool _disposed;

    protected ViewModel()
    {
        ExtraProperties = new ExtraPropertyDictionary();
        this.SetDefaultsForExtraProperties();
    }

    public required IServiceProvider ServiceProvider { protected get; init; }

    public required IAbpLazyServiceProvider LazyServiceProvider { protected get; init; }

    protected IClock Clock => LazyServiceProvider.LazyGetRequiredService<IClock>();

    protected ILoggerFactory LoggerFactory =>
        LazyServiceProvider.LazyGetRequiredService<ILoggerFactory>();

    protected ILogger Logger =>
        LazyServiceProvider.LazyGetService<ILogger>(_ =>
            LoggerFactory.CreateLogger(GetType().FullName!)
        );

    protected IMessenger Messenger => ServiceProvider.GetRequiredService<IMessenger>();

    protected IToastService ToastService => ServiceProvider.GetRequiredService<IToastService>();

    protected IDialogService DialogService => ServiceProvider.GetRequiredService<IDialogService>();

    protected SettingsService SettingsService =>
        ServiceProvider.GetRequiredService<SettingsService>();

    protected IThemeService ThemeService => ServiceProvider.GetRequiredService<IThemeService>();

    public GeneralSetting GeneralSetting => SettingsService.Get<GeneralSetting>();

    public AppearanceSetting AppearanceSetting => SettingsService.Get<AppearanceSetting>();

    public LoggingSetting LoggingSetting => SettingsService.Get<LoggingSetting>();

    public IStorageProvider StorageProvider =>
        ServiceProvider.GetRequiredService<IStorageProvider>();

    public IClipboard Clipboard => ServiceProvider.GetRequiredService<IClipboard>();
    public ILauncher Launcher => ServiceProvider.GetRequiredService<ILauncher>();

    public ExtraPropertyDictionary ExtraProperties { get; protected set; }

    [ObservableProperty]
    public partial bool IsBusy { get; set; }

    [ObservableProperty]
    public partial string IsBusyText { get; set; } = string.Empty;

    public virtual void OnLoaded() { }

    public virtual void OnUnloaded() { }

    protected void OnAllPropertiesChanged() => OnPropertyChanged(string.Empty);

    protected async Task SetBusyAsync(
        Func<Task> func,
        string busyText = "",
        bool showException = true
    )
    {
        IsBusy = true;
        IsBusyText = busyText;
        try
        {
            await func();
        }
        catch (Exception ex) when (LogException(ex, true, showException))
        {
            // Not Used
        }
        finally
        {
            IsBusy = false;
            IsBusyText = string.Empty;
        }
    }

    protected bool LogException(Exception? ex, bool shouldCatch = false, bool shouldDisplay = false)
    {
        if (ex is null)
        {
            return shouldCatch;
        }

        Logger.LogException(ex);
        if (shouldDisplay)
        {
            ToastService.ShowExceptionToast(ex, "Error", ex.ToStringDemystified());
        }

        return shouldCatch;
    }

    protected virtual bool CanExecuteShowPage() => true;

    protected virtual Task ShowPageAsync<TViewModel>()
        where TViewModel : ViewModel => ShowPageAsync(typeof(TViewModel));

    [RelayCommand(CanExecute = nameof(CanExecuteShowPage))]
    protected virtual Task ShowPageAsync(Type pageType)
    {
        Messenger.Send(new ShowPageMessage(pageType));
        return Task.CompletedTask;
    }

    #region Disposal

    ~ViewModel() => Dispose(false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="Dispose"/>>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            var disposables = this.GetProperty("Disposables") as CompositeDisposable;
            disposables?.Dispose();
        }

        _disposed = true;
    }

    #endregion
}
