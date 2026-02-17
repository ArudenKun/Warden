using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.DependencyInjection;
using SukiUI.Controls;
using Warden.Utilities;
using Warden.ViewModels;

namespace Warden.Views;

public abstract class SukiWindow<TViewModel> : SukiWindow, IView<TViewModel>
    where TViewModel : ViewModel
{
    protected SukiWindow()
    {
#if DEBUG
        if (Design.IsDesignMode)
        {
            DataContext = Ioc.Default.GetRequiredService<TViewModel>();
        }
#endif
    }

    public new TViewModel DataContext
    {
        get =>
            base.DataContext as TViewModel
            ?? throw new InvalidCastException(
                $"DataContext is null or not of the expected type '{typeof(TViewModel).FullName}'."
            );
        set => base.DataContext = value;
    }

    public TViewModel ViewModel => DataContext;

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        DispatchHelper.Invoke(() => ViewModel.OnLoaded());
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        DispatchHelper.Invoke(() => ViewModel.OnUnloaded());
    }
}
