using System.Collections.Concurrent;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Volo.Abp.DependencyInjection;

namespace Warden.Core;

public class ViewLocator : IDataTemplate, ISingletonDependency
{
    private static readonly Type ViewInterfaceType = typeof(IView<>);
    private static readonly ConcurrentDictionary<Type, Type> Cache = new();

    private readonly IServiceProvider _serviceProvider;

    public ViewLocator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
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
        var viewType = Cache.GetOrAdd(viewModelType, k => ViewInterfaceType.MakeGenericType(k));

        if (_serviceProvider.GetService(viewType) is not Control view)
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
}
