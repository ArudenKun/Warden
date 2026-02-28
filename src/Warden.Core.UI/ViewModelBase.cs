using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using R3;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;

namespace Warden.Core;

[PublicAPI]
public abstract partial class ViewModelBase
    : ObservableValidator,
        IDisposable,
        ITransientDependency,
        IHasExtraProperties
{
    private bool _disposed;

    protected ViewModelBase()
    {
        ExtraProperties = new ExtraPropertyDictionary();
        this.SetDefaultsForExtraProperties();
    }

    public required IServiceProvider ServiceProvider { protected get; init; }

    public required IAbpLazyServiceProvider LazyServiceProvider { protected get; init; }

    protected IClock Clock => LazyServiceProvider.LazyGetRequiredService<IClock>();

    protected ILoggerFactory LoggerFactory =>
        LazyServiceProvider.LazyGetRequiredService<ILoggerFactory>();

    protected ILogger Logger =>
        LazyServiceProvider.LazyGetService<ILogger>(_ =>
            LoggerFactory.CreateLogger(GetType().FullName!)
        );

    protected IMessenger Messenger => ServiceProvider.GetRequiredService<IMessenger>();

    public ExtraPropertyDictionary ExtraProperties { get; protected set; }

    [ObservableProperty]
    public partial bool IsBusy { get; set; }

    [ObservableProperty]
    public partial string IsBusyText { get; set; } = string.Empty;

    public virtual void OnLoaded() { }

    public virtual void OnUnloaded() { }

    protected void OnAllPropertiesChanged() => OnPropertyChanged(string.Empty);

    protected virtual async Task SetBusyAsync(
        Func<Task> func,
        string busyText = "",
        bool showException = true
    )
    {
        IsBusy = true;
        IsBusyText = busyText;
        try
        {
            await func();
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
        }
        finally
        {
            IsBusy = false;
            IsBusyText = string.Empty;
        }
    }

    #region Disposal

    ~ViewModelBase() => Dispose(false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="Dispose"/>>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            var disposables = this.GetProperty("Disposables") as CompositeDisposable;
            disposables?.Dispose();
        }

        _disposed = true;
    }

    #endregion
}
