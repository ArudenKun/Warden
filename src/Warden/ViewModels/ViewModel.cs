using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Warden.Core;
using Warden.Core.Navigation;
using Warden.Core.Settings;
using Warden.Options;
using Warden.Services;

namespace Warden.ViewModels;

[PublicAPI]
public abstract partial class ViewModel : ViewModelBase
{
    protected IToastService ToastService => ServiceProvider.GetRequiredService<IToastService>();

    protected IDialogService DialogService => ServiceProvider.GetRequiredService<IDialogService>();

    protected ISettingsService SettingsService =>
        ServiceProvider.GetRequiredService<ISettingsService>();

    protected IThemeService ThemeService => ServiceProvider.GetRequiredService<IThemeService>();

    public GeneralOptions GeneralOptions => SettingsService.Get<GeneralOptions>();

    public AppearanceOptions AppearanceOptions => SettingsService.Get<AppearanceOptions>();

    public LoggingOptions LoggingOptions => SettingsService.Get<LoggingOptions>();

    public IStorageProvider StorageProvider =>
        ServiceProvider.GetRequiredService<IStorageProvider>();

    public IClipboard Clipboard => ServiceProvider.GetRequiredService<IClipboard>();
    public ILauncher Launcher => ServiceProvider.GetRequiredService<ILauncher>();

    public INavigationHostManager NavigationHostManager =>
        LazyServiceProvider.GetRequiredService<INavigationHostManager>();

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

    protected virtual Task ShowPageAsync<TView>()
        where TView : Control => ShowPageAsync(typeof(TView));

    [RelayCommand(CanExecute = nameof(CanExecuteShowPage))]
    protected virtual Task ShowPageAsync(Type pageType)
    {
        NavigationHostManager.Navigate(Regions.Main, pageType);
        return Task.CompletedTask;
    }
}
