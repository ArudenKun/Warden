using Warden.Core.Histories.Internal;

namespace Warden.Core.Histories.Extensions;

/// <summary>
/// Provides a method to wrap an <see cref="IDictionary{TKey, TValue}"/> to an UnDo dictionary linked to an <see cref="IUndoManager"/> to automatically generate <see cref="IUndo"/> operations.
/// </summary>
public static class IDictionaryExtensions
{
    /// <summary>
    ///  Wraps an <see cref="IDictionary{TKey, TValue}"/> to an UnDo dictionary linked to an <see cref="IUndoManager"/> to automatically generate <see cref="IUndo"/> operations.
    /// </summary>
    /// <typeparam name="TKey">The type of keys.</typeparam>
    /// <typeparam name="TValue">The type of values.</typeparam>
    /// <param name="source">The <see cref="IDictionary{TKey, TValue}"/>.</param>
    /// <param name="manager">The <see cref="IUndoManager"/>.</param>
    /// <param name="descriptionFactory">Factory used to create the description of the generated <see cref="IUndo"/>.</param>
    /// <returns>A wrapped <see cref="IDictionary{TKey, TValue}"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="manager"/> is null.</exception>
    public static IDictionary<TKey, TValue> AsUndo<TKey, TValue>(
        this IDictionary<TKey, TValue> source,
        IUndoManager manager,
        Func<UndoCollectionOperation, object?>? descriptionFactory = null
    )
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(manager);

        return new UndoIDictionary<TKey, TValue>(manager, source, descriptionFactory);
    }
}
