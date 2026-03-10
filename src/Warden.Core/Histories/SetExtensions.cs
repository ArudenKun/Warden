using Warden.Core.Histories.Internals;

namespace Warden.Core.Histories;

/// <summary>
/// Provides a method to wrap an <see cref="ISet{T}"/> to an UnDo set linked to an <see cref="IHistory"/> to automatically generate <see cref="IUnDo"/> operations.
/// </summary>
public static class SetExtensions
{
    /// <summary>
    ///  Wraps an <see cref="ISet{T}"/> to an UnDo set linked to an <see cref="IHistory"/> to automatically generate <see cref="IUnDo"/> operations.
    /// </summary>
    /// <typeparam name="T">The type of element in the <see cref="ISet{T}"/>.</typeparam>
    /// <param name="source">The <see cref="ISet{T}"/>.</param>
    /// <param name="history">The <see cref="IHistory"/>.</param>
    /// <param name="descriptionFactory">Factory used to create the description of the generated <see cref="IUnDo"/>.</param>
    /// <returns>A wrapped <see cref="ISet{T}"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="history"/> is null.</exception>
    public static ISet<T> AsUnDo<T>(
        this ISet<T> source,
        IHistory history,
        Func<UnDoCollectionOperation, object?>? descriptionFactory = null
    )
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(history);

        return new UndoSet<T>(history, source, descriptionFactory);
    }
}
