using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace Warden.Core;

public class ViewLocator : IDataTemplate, ISingletonDependency
{
    private const string Key = "ViewLocator";

    private static readonly Type ViewInterfaceType = typeof(IView<>);

    private readonly IServiceProvider _serviceProvider;
    private readonly IDistributedCache<Type> _cache;

    public ViewLocator(IServiceProvider serviceProvider, IDistributedCache<Type> cache)
    {
        _serviceProvider = serviceProvider;
        _cache = cache;
    }

    public TView CreateView<TView, TViewModel>(TViewModel viewModel)
        where TView : Control
        where TViewModel : ViewModelBase
    {
        return (TView)CreateView(viewModel);
    }

    public Control CreateView(ViewModelBase viewModel)
    {
        var viewModelType = viewModel.GetType();
        var viewType = _cache.GetOrAdd(
            GetKey(viewModelType),
            () => ViewInterfaceType.MakeGenericType(viewModelType)
        );

        if (viewType is null || _serviceProvider.GetService(viewType) is not Control view)
            return CreateText($"Could not find view for {viewModelType.FullName}");

        view.DataContext = viewModel;
        return view;
    }

    Control ITemplate<object?, Control?>.Build(object? data)
    {
        if (data is ViewModelBase viewModel)
        {
            return CreateView(viewModel);
        }

        return CreateText($"Could not find view for {data?.GetType().FullName}");
    }

    bool IDataTemplate.Match(object? data) => data is ViewModelBase;

    private static TextBlock CreateText(string text) => new() { Text = text };

    private static string GetKey(Type viewModelType) => $"{viewModelType.FullName}:{Key}";
}
