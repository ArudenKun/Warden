using Warden.Core.Histories.Internals;

namespace Warden.Core.Histories;

/// <summary>
/// Provides a method to wrap an <see cref="IDictionary{TKey, TValue}"/> to an UnDo dictionary linked to an <see cref="IHistory"/> to automatically generate <see cref="IUnDo"/> operations.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    ///  Wraps an <see cref="IDictionary{TKey, TValue}"/> to an UnDo dictionary linked to an <see cref="IHistory"/> to automatically generate <see cref="IUnDo"/> operations.
    /// </summary>
    /// <typeparam name="TKey">The type of keys.</typeparam>
    /// <typeparam name="TValue">The type of values.</typeparam>
    /// <param name="source">The <see cref="IDictionary{TKey, TValue}"/>.</param>
    /// <param name="history">The <see cref="IHistory"/>.</param>
    /// <param name="descriptionFactory">Factory used to create the description of the generated <see cref="IUnDo"/>.</param>
    /// <returns>A wrapped <see cref="IDictionary{TKey, TValue}"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="history"/> is null.</exception>
    public static IDictionary<TKey, TValue> AsUndo<TKey, TValue>(
        this IDictionary<TKey, TValue> source,
        IHistory history,
        Func<UnDoCollectionOperation, object?>? descriptionFactory = null
    )
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(history);

        return new UndoDictionary<TKey, TValue>(history, source, descriptionFactory);
    }
}
