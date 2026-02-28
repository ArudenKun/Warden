using System.Diagnostics;
using Avalonia.Input.Platform;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Warden.Core;
using Warden.Messaging.Messages;
using Warden.Services;
using Warden.Services.Settings;
using Warden.Settings;

namespace Warden.ViewModels;

[PublicAPI]
public abstract partial class ViewModel : ViewModelBase
{
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

    protected override async Task SetBusyAsync(
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
}
