using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ServiceScan.SourceGenerator;
using Warden.ViewModels;
using Warden.Views;

namespace Warden;

public sealed partial class ViewLocator : IDataTemplate
{
    private static readonly Dictionary<Type, Func<Control>> ViewTypeCache = new();

    public ViewLocator()
    {
        RegisterViews();
    }

    public TView CreateView<TView, TViewModel>(TViewModel viewModel)
        where TView : Control
        where TViewModel : ViewModel
    {
        return (TView)CreateView(viewModel);
    }

    public Control CreateView(ViewModel viewModel)
    {
        var viewModelType = viewModel.GetType();
        var viewFactory = ViewTypeCache.GetValueOrDefault(viewModelType);
        if (viewFactory is null)
            return CreateText($"Could not find view for {viewModelType.FullName}");

        var view = viewFactory();
        view.DataContext = viewModel;
        return view;
    }

    Control ITemplate<object?, Control?>.Build(object? data)
    {
        if (data is ViewModel viewModel)
        {
            return CreateView(viewModel);
        }

        return CreateText($"Could not find view for {data?.GetType().FullName}");
    }

    bool IDataTemplate.Match(object? data) => data is ViewModel;

    private static TextBlock CreateText(string text) => new() { Text = text };

    [GenerateServiceRegistrations(
        AssignableTo = typeof(IView<>),
        CustomHandler = nameof(RegisterViewsHandler)
    )]
    private partial void RegisterViews();

    private static void RegisterViewsHandler<TView, TViewModel>()
        where TView : Control, IView<TViewModel>, new()
        where TViewModel : ViewModel => ViewTypeCache.TryAdd(typeof(TViewModel), () => new TView());
}
