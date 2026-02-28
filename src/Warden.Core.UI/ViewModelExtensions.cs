using R3;
using Volo.Abp.Data;

namespace Warden.Core;

public static class ViewModelExtensions
{
    public static TDisposable AddTo<TDisposable>(
        this TDisposable disposable,
        ViewModelBase viewModel
    )
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
