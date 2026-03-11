using Warden.Core.Histories.Internal;

namespace Warden.Core.Histories.Extensions;

/// <summary>
/// Provides a method to wrap an <see cref="ICollection{T}"/> to an UnDo collection linked to an <see cref="IUndoManager"/> to automatically generate <see cref="IUndo"/> operations.
/// </summary>
public static class ICollectionExtensions
{
    /// <summary>
    ///  Wraps an <see cref="ICollection{T}"/> to an UnDo collection linked to an <see cref="IUndoManager"/> to automatically generate <see cref="IUndo"/> operations.
    /// </summary>
    /// <typeparam name="T">The type of element in the <see cref="ICollection{T}"/>.</typeparam>
    /// <param name="source">The <see cref="ICollection{T}"/>.</param>
    /// <param name="manager">The <see cref="IUndoManager"/>.</param>
    /// <param name="descriptionFactory">Factory used to create the description of the generated <see cref="IUndo"/>.</param>
    /// <returns>A wrapped <see cref="ICollection{T}"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="manager"/> is null.</exception>
    public static ICollection<T> AsUndo<T>(
        this ICollection<T> source,
        IUndoManager manager,
        Func<UndoCollectionOperation, object?>? descriptionFactory = null
    )
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(source);

        return new UndoICollection<T>(manager, source, descriptionFactory);
    }
}
