using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Volo.Abp;
using Warden.Abp;
using Warden.ViewModels;
using Warden.Views;
using ZLinq;

namespace Warden;

public sealed class App : AbpAvaloniaApplication<WardenModule, MainWindow>
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    protected override void ConfigureAbpCreationOptions(AbpApplicationCreationOptions options)
    {
        options.UseAutofac();
        options.Services.AddLogging(builder => builder.ClearProviders().AddSerilog(dispose: true));
    }

    protected override MainWindow CreateWindow(IServiceProvider serviceProvider)
    {
        DisableAvaloniaDataAnnotationValidation();
        return (
            DataTemplates[0].Build(serviceProvider.GetRequiredService<MainWindowViewModel>())
            as MainWindow
        )!;
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
}
