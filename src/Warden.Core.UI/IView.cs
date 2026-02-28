namespace Warden.Core;

public interface IView<TViewModel>
    where TViewModel : ViewModelBase
{
    TViewModel ViewModel { get; }
    TViewModel DataContext { get; set; }
}
