using R3;
using Volo.Abp.Data;

namespace Warden.ViewModels;

public static class ViewModelExtensions
{
    public static TDisposable AddTo<TDisposable>(this TDisposable disposable, ViewModel viewModel)
        where TDisposable : IDisposable
    {
        if (viewModel.GetProperty("Disposables") is not CompositeDisposable disposables)
        {
            disposables = new CompositeDisposable();
            viewModel.SetProperty("Disposables", disposables);
        }

        disposables.Add(disposable);
        return disposable;
    }
}
