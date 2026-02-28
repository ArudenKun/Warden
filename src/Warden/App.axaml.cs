using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using R3;
using R3.ObservableEvents;
using Volo.Abp.DependencyInjection;
using Warden.Services;
using Warden.Services.Settings;
using Warden.Settings;
using Warden.Utilities;
using Warden.ViewModels;
using ZLinq;

namespace Warden;

public sealed class App : Application, IDisposable, ISingletonDependency
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILoggerFactory _loggerFactory;

    private IDisposable? _subscriptions;

    public App(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
    {
        _serviceProvider = serviceProvider;
        _loggerFactory = loggerFactory;
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        var settingsService = _serviceProvider.GetRequiredService<ISettingsService>();
        var themeService = _serviceProvider.GetRequiredService<IThemeService>();
        var loggingSetting = settingsService.Get<LoggingSetting>();
        LogHelper.Initialize(loggingSetting);
        themeService.Initialize();

        _subscriptions = Disposable.Combine(
            AppDomain
                .CurrentDomain.Events()
                .UnhandledException.Subscribe(e =>
                    HandleUnhandledException((Exception)e.ExceptionObject, AppConsts.Name)
                ),
            RxEvents.TaskSchedulerUnobservedTaskException.Subscribe(e =>
            {
                HandleUnhandledException(e.Exception, "Task");
                e.SetObserved();
            }),
            Dispatcher
                .UIThread.Events()
                .UnhandledException.Subscribe(e =>
                {
                    HandleUnhandledException(e.Exception, "UI");
                    e.Handled = true;
                })
        );
    }

    public override void OnFrameworkInitializationCompleted()
    {
        DisableAvaloniaDataAnnotationValidation();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _serviceProvider.GetRequiredService<ObjectAccessor<TopLevel>>().Value =
                desktop.MainWindow =
                    DataTemplates[0]
                        .Build(_serviceProvider.GetRequiredService<MainWindowViewModel>())
                    as Window;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove = BindingPlugins
            .DataValidators.AsValueEnumerable()
            .OfType<DataAnnotationsValidationPlugin>()
            .ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

    public void Dispose() => _subscriptions?.Dispose();

    private void HandleUnhandledException(Exception exception, string category)
    {
        var toastService = _serviceProvider.GetRequiredService<IToastService>();
        var logger = _loggerFactory.CreateLogger(category);
        logger.LogError(exception, "Unhandled Exception");
        DispatchHelper.Invoke(() =>
            toastService.ShowExceptionToast(exception, $"{category} Exception")
        );
    }
}
