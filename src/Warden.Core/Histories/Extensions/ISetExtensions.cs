using Warden.Core.Histories.Internal;

namespace Warden.Core.Histories.Extensions;

/// <summary>
/// Provides a method to wrap an <see cref="ISet{T}"/> to an UnDo set linked to an <see cref="IUndoManager"/> to automatically generate <see cref="IUndo"/> operations.
/// </summary>
public static class ISetExtensions
{
    /// <summary>
    ///  Wraps an <see cref="ISet{T}"/> to an UnDo set linked to an <see cref="IUndoManager"/> to automatically generate <see cref="IUndo"/> operations.
    /// </summary>
    /// <typeparam name="T">The type of element in the <see cref="ISet{T}"/>.</typeparam>
    /// <param name="source">The <see cref="ISet{T}"/>.</param>
    /// <param name="manager">The <see cref="IUndoManager"/>.</param>
    /// <param name="descriptionFactory">Factory used to create the description of the generated <see cref="IUndo"/>.</param>
    /// <returns>A wrapped <see cref="ISet{T}"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="manager"/> is null.</exception>
    public static ISet<T> AsUndo<T>(
        this ISet<T> source,
        IUndoManager manager,
        Func<UndoCollectionOperation, object?>? descriptionFactory = null
    )
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(manager);

        return new UndoISet<T>(manager, source, descriptionFactory);
    }
}
