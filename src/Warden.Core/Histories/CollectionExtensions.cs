using Warden.Core.Histories.Internals;

namespace Warden.Core.Histories;

/// <summary>
/// Provides a method to wrap an <see cref="ICollection{T}"/> to an UnDo collection linked to an <see cref="IHistory"/> to automatically generate <see cref="IUndo"/> operations.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    ///  Wraps an <see cref="ICollection{T}"/> to an UnDo collection linked to an <see cref="IHistory"/> to automatically generate <see cref="IUndo"/> operations.
    /// </summary>
    /// <typeparam name="T">The type of element in the <see cref="ICollection{T}"/>.</typeparam>
    /// <param name="source">The <see cref="ICollection{T}"/>.</param>
    /// <param name="history">The <see cref="IHistory"/>.</param>
    /// <param name="descriptionFactory">Factory used to create the description of the generated <see cref="IUndo"/>.</param>
    /// <returns>A wrapped <see cref="ICollection{T}"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="history"/> is null.</exception>
    public static ICollection<T> AsUndo<T>(
        this ICollection<T> source,
        IHistory history,
        Func<UnDoCollectionOperation, object?>? descriptionFactory = null
    )
    {
        ArgumentNullException.ThrowIfNull(history);
        ArgumentNullException.ThrowIfNull(source);

        return new UndoCollection<T>(history, source, descriptionFactory);
    }
}
