using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace Warden.Abp;

public abstract class AbpAvaloniaApplication<TStartupModule, TMainWindow> : Application
    where TStartupModule : AbpModule
    where TMainWindow : Window
{
    private IAbpApplicationWithInternalServiceProvider? _abpApplication;
    private TMainWindow? _mainWindow;

    protected TMainWindow MainWindow =>
        _mainWindow ?? throw new InvalidOperationException("Application is not yet initialized");

    protected virtual void ConfigureAbpCreationOptions(AbpApplicationCreationOptions options) { }

    protected abstract TMainWindow CreateWindow(IServiceProvider serviceProvider);

    public sealed override void OnFrameworkInitializationCompleted()
    {
        if (_abpApplication is not null)
            return;

        _abpApplication = AbpApplicationFactory.Create<TStartupModule>(ConfigureAbpCreationOptions);
        _abpApplication.Services.AddSingleton(sp => new MainWindowWrapper(CreateWindow(sp)));
        _abpApplication.Services.AddSingleton<TopLevel>(sp =>
            sp.GetRequiredService<MainWindowWrapper>().Window
        );
        _abpApplication.Initialize();
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
            return;

        desktop.Exit += (_, _) =>
        {
            _abpApplication.Shutdown();
            _abpApplication.Dispose();
            _abpApplication = null;
        };

        desktop.MainWindow = _mainWindow = _abpApplication
            .ServiceProvider.GetRequiredService<MainWindowWrapper>()
            .Window;
    }

    private class MainWindowWrapper
    {
        public TMainWindow Window { get; }

        public MainWindowWrapper(TMainWindow window)
        {
            Window = window;
        }
    }
}
