using R3;

namespace Warden.Core.Histories.R3;

/// <summary>
/// Observable extension methods for the generic observable implementations.
/// </summary>
public static class ObservableExtensions
{
    /// <summary>
    /// Observe property changes with history.
    /// </summary>
    /// <param name="source">The property value observable.</param>
    /// <param name="update">The property update action.</param>
    /// <param name="currentValue">The property current value.</param>
    /// <param name="history">The history object.</param>
    /// <returns>The property value changes subscription.</returns>
    public static IDisposable ObserveWithHistory<T>(
        this Observable<T> source,
        Action<T> update,
        T currentValue,
        IHistory history
    )
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(update);
        ArgumentNullException.ThrowIfNull(history);

        var previous = currentValue;

        return source
            .Skip(1)
            .Subscribe(next =>
            {
                if (!history.IsPaused)
                {
                    var undoValue = previous;
                    var redoValue = next;
                    void Undo() => update(undoValue);
                    void Redo() => update(redoValue);
                    history.Snapshot(Undo, Redo);
                }
                previous = next;
            });
    }
}
