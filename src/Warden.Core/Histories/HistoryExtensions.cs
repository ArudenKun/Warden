namespace Warden.Core.Histories;

/// <summary>
/// Provides methods to create <see cref="IUndo"/> command and add them to an <see cref="IHistory"/>.
/// </summary>
public static class HistoryExtensions
{
    /// <summary>
    /// Adds an item from a <see cref="ISet{T}"/> as a <see cref="IUndo"/> operation.
    /// </summary>
    /// <typeparam name="T">The type of element in the <see cref="ISet{T}"/>.</typeparam>
    /// <param name="history">The <see cref="IHistory"/>.</param>
    /// <param name="source">The <see cref="ISet{T}"/>.</param>
    /// <param name="item">The item to add.</param>
    /// <param name="description">The description of the operation.</param>
    /// <returns>true if the command has been created, false if not because <paramref name="source"/> already contained <paramref name="item"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="history"/> or <paramref name="source"/> is null.</exception>
    public static bool ExecuteAdd<T>(
        this IHistory history,
        ISet<T> source,
        T? item,
        object? description = null
    )
    {
        ArgumentNullException.ThrowIfNull(history);
        ArgumentNullException.ThrowIfNull(source);

        bool result = !source.Contains(item!);

        if (result)
        {
            history.Execute(new CollectionUndo<T>(description, source, item, true));
        }

        return result;
    }

    /// <summary>
    /// Adds a value to a <see cref="ICollection{T}"/> as a <see cref="IUndo"/> operation.
    /// </summary>
    /// <typeparam name="T">The type of element in the <see cref="ICollection{T}"/>.</typeparam>
    /// <param name="history">The <see cref="IHistory"/>.</param>
    /// <param name="source">The <see cref="ICollection{T}"/>.</param>
    /// <param name="item">The item to add.</param>
    /// <param name="description">The description of the operation.</param>
    /// <exception cref="ArgumentNullException"><paramref name="history"/> or <paramref name="source"/> is null.</exception>
    public static void ExecuteAdd<T>(
        this IHistory history,
        ICollection<T> source,
        T? item,
        object? description = null
    )
    {
        ArgumentNullException.ThrowIfNull(history);
        ArgumentNullException.ThrowIfNull(source);

        if (source is IList<T> list)
        {
            history.ExecuteInsert(list, list.Count, item, description);
        }
        else
        {
            history.Execute(new CollectionUndo<T>(description, source, item, true));
        }
    }

    /// <summary>
    /// Clears a <see cref="ICollection{T}"/> as a <see cref="IUndo"/> operation.
    /// </summary>
    /// <typeparam name="T">The type of element in the <see cref="ICollection{T}"/>.</typeparam>
    /// <param name="history">The <see cref="IHistory"/>.</param>
    /// <param name="source">The <see cref="ICollection{T}"/>.</param>
    /// <param name="description">The description of the operation.</param>
    /// <exception cref="ArgumentNullException"><paramref name="history"/> or <paramref name="source"/> is null.</exception>
    public static void ExecuteClear<T>(
        this IHistory history,
        ICollection<T> source,
        object? description = null
    )
    {
        ArgumentNullException.ThrowIfNull(history);
        ArgumentNullException.ThrowIfNull(source);

        if (source.Count > 0)
        {
            T[] oldValues = [.. source];

            history.Execute(
                source.Clear,
                () =>
                {
                    foreach (T oldValue in oldValues)
                    {
                        source.Add(oldValue);
                    }
                },
                description
            );
        }
    }

    /// <summary>
    /// Removes an item from a <see cref="ICollection{T}"/> as a <see cref="IUndo"/> operation.
    /// </summary>
    /// <typeparam name="T">The type of element in the <see cref="ICollection{T}"/>.</typeparam>
    /// <param name="history">The <see cref="IHistory"/>.</param>
    /// <param name="source">The <see cref="ICollection{T}"/>.</param>
    /// <param name="item">The item to remove.</param>
    /// <param name="description">The description of the operation.</param>
    /// <returns>true if the command has been created, false if not because <paramref name="source"/> did not contained <paramref name="item"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="history"/> or <paramref name="source"/> is null.</exception>
    public static bool ExecuteRemove<T>(
        this IHistory history,
        ICollection<T> source,
        T? item,
        object? description = null
    )
    {
        ArgumentNullException.ThrowIfNull(history);
        ArgumentNullException.ThrowIfNull(source);

        bool result = false;

        if (source is IList<T> list)
        {
            int index = list.IndexOf(item!);
            if (index >= 0)
            {
                history.ExecuteRemoveAt(list, index, description);
                result = true;
            }
        }
        else if (source.Contains(item!))
        {
            history.Execute(new CollectionUndo<T>(description, source, item, false));
            result = true;
        }

        return result;
    }

    /// <summary>
    /// Adds a value to a <see cref="IDictionary{TKey, TValue}"/> as a <see cref="IUndo"/> operation.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="history">The <see cref="IHistory"/>.</param>
    /// <param name="source">The <see cref="IDictionary{TKey, TValue}"/>.</param>
    /// <param name="key">The key to add.</param>
    /// <param name="value">The value to add.</param>
    /// <param name="description">The description of the operation.</param>
    /// <exception cref="ArgumentNullException"><paramref name="history"/>, <paramref name="source"/> or <paramref name="key"/> is null.</exception>
    public static void ExecuteAdd<TKey, TValue>(
        this IHistory history,
        IDictionary<TKey, TValue> source,
        TKey key,
        TValue? value,
        object? description = null
    )
    {
        ArgumentNullException.ThrowIfNull(history);
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(key);

        history.Execute(new DictionaryUndo<TKey, TValue>(description, source, key, value, true));
    }

    /// <summary>
    /// Removes the item with the specified key from a <see cref="IDictionary{TKey, TValue}"/> as a <see cref="IUndo"/> operation.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="history">The <see cref="IHistory"/>.</param>
    /// <param name="source">The <see cref="IDictionary{TKey, TValue}"/>.</param>
    /// <param name="key">The key to remove.</param>
    /// <param name="description">The description of the operation.</param>
    /// <returns>true if the command has been created, false if not because <paramref name="source"/> did not contained <paramref name="key"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="history"/>, <paramref name="source"/> or <paramref name="key"/> is null.</exception>
    public static bool ExecuteRemove<TKey, TValue>(
        this IHistory history,
        IDictionary<TKey, TValue> source,
        TKey key,
        object? description = null
    )
    {
        ArgumentNullException.ThrowIfNull(history);
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(key);

        bool result = false;
        if (source.TryGetValue(key, out TValue? value))
        {
            history.Execute(
                new DictionaryUndo<TKey, TValue>(description, source, key, value, false)
            );
            result = true;
        }

        return result;
    }

    /// <summary>
    /// Sets the element with the specified key on a <see cref="IDictionary{TKey, TValue}"/> as a <see cref="IUndo"/> operation.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="history">The <see cref="IHistory"/>.</param>
    /// <param name="source">The <see cref="IDictionary{TKey, TValue}"/>.</param>
    /// <param name="key">The key to set.</param>
    /// <param name="value">The value to add.</param>
    /// <param name="description">The description of the operation.</param>
    /// <exception cref="ArgumentNullException"><paramref name="history"/>, <paramref name="source"/> or <paramref name="key"/> is null.</exception>
    public static void Execute<TKey, TValue>(
        this IHistory history,
        IDictionary<TKey, TValue> source,
        TKey key,
        TValue? value,
        object? description = null
    )
    {
        ArgumentNullException.ThrowIfNull(history);
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(key);

        if (source.TryGetValue(key, out TValue? oldValue))
        {
            history.Execute(v => source[key] = v!, value, oldValue, description);
        }
        else
        {
            history.Execute(() => source[key] = value!, () => source.Remove(key), description);
        }
    }

    /// <summary>
    /// Inserts an item to a <see cref="IList{T}"/> at the specified index as a <see cref="IUndo"/> operation.
    /// </summary>
    /// <typeparam name="T">The type of element in the <see cref="IList{T}"/>.</typeparam>
    /// <param name="history">The <see cref="IHistory"/>.</param>
    /// <param name="source">The <see cref="IList{T}"/>.</param>
    /// <param name="index">The zero-based index at which item should be inserted.</param>
    /// <param name="item">The item to insert into the <see cref="IList{T}"/>.</param>
    /// <param name="description">The description of the operation.</param>
    /// <exception cref="ArgumentNullException"><paramref name="history"/> or <paramref name="source"/> is null.</exception>
    public static void ExecuteInsert<T>(
        this IHistory history,
        IList<T> source,
        int index,
        T? item,
        object? description = null
    )
    {
        ArgumentNullException.ThrowIfNull(history);
        ArgumentNullException.ThrowIfNull(source);

        history.Execute(new ListUndo<T>(description, source, index, item, true));
    }

    /// <summary>
    /// Removes an item from a <see cref="IList{T}"/> at the specified index as a <see cref="IUndo"/> operation.
    /// </summary>
    /// <typeparam name="T">The type of element in the <see cref="IList{T}"/>.</typeparam>
    /// <param name="history">The <see cref="IHistory"/>.</param>
    /// <param name="source">The <see cref="IList{T}"/>.</param>
    /// <param name="index">The zero-based index at which item should be removed.</param>
    /// <param name="description">The description of the operation.</param>
    /// <exception cref="ArgumentNullException"><paramref name="history"/> or <paramref name="source"/> is null.</exception>
    public static void ExecuteRemoveAt<T>(
        this IHistory history,
        IList<T> source,
        int index,
        object? description = null
    )
    {
        ArgumentNullException.ThrowIfNull(history);
        ArgumentNullException.ThrowIfNull(source);

        history.Execute(new ListUndo<T>(description, source, index, source[index], false));
    }

    /// <summary>
    /// Sets the element at the specified index on a <see cref="IList{T}"/> as a <see cref="IUndo"/> operation.
    /// </summary>
    /// <typeparam name="T">The type of element in the <see cref="IList{T}"/>.</typeparam>
    /// <param name="history">The <see cref="IHistory"/>.</param>
    /// <param name="source">The <see cref="IList{T}"/>.</param>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <param name="item">The new item.</param>
    /// <param name="description">The description of the operation.</param>
    /// <exception cref="ArgumentNullException"><paramref name="history"/> or <paramref name="source"/> is null.</exception>
    public static void Execute<T>(
        this IHistory history,
        IList<T> source,
        int index,
        T? item,
        object? description = null
    )
    {
        ArgumentNullException.ThrowIfNull(history);
        ArgumentNullException.ThrowIfNull(source);

        history.Execute(v => source[index] = v!, item, source[index], description);
    }

    /// <summary>
    /// Does a <see cref="IUndo"/> operation on the manager with the specified doAction and undoAction.
    /// </summary>
    /// <param name="history">The <see cref="IHistory"/>.</param>
    /// <param name="executeAction">The <see cref="Action"/> performed by <see cref="IUndo.Execute"/>.</param>
    /// <param name="undoAction">The <see cref="Action"/> performed by the <see cref="IUndo.Undo"/>.</param>
    /// <param name="description">The description of the operation.</param>
    /// <exception cref="ArgumentNullException"><paramref name="history"/> is null.</exception>
    public static void Execute(
        this IHistory history,
        Action? executeAction,
        Action? undoAction,
        object? description = null
    )
    {
        ArgumentNullException.ThrowIfNull(history);

        if (executeAction != null || undoAction != null)
        {
            history.Execute(new Undo(description, executeAction, undoAction));
        }
    }

    /// <summary>
    /// Does a <see cref="IUndo"/> operation on the manager with the specified action with no undo.
    /// </summary>
    /// <param name="history">The <see cref="IHistory"/>.</param>
    /// <param name="action">The <see cref="Action"/> performed by <see cref="IUndo.Execute"/>.</param>
    /// <param name="description">The description of the operation.</param>
    /// <exception cref="ArgumentNullException"><paramref name="history"/> is null.</exception>
    public static void ExecuteOnExecute(
        this IHistory history,
        Action action,
        object? description = null
    ) => history.Execute(action, null, description);

    /// <summary>
    /// Does a <see cref="IUndo"/> operation on the manager with the specified action with no do.
    /// </summary>
    /// <param name="history">The <see cref="IHistory"/>.</param>
    /// <param name="action">The <see cref="Action"/> performed by <see cref="IUndo.Undo"/>.</param>
    /// <param name="description">The description of the operation.</param>
    /// <exception cref="ArgumentNullException"><paramref name="history"/> is null.</exception>
    public static void ExecuteOnUndo(
        this IHistory history,
        Action action,
        object? description = null
    ) => history.Execute(null, action, description);

    /// <summary>
    /// Sets a value as a <see cref="IUndo"/> operation.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="history">The <see cref="IHistory"/>.</param>
    /// <param name="setter">The <see cref="Action{T}"/> used to change the value.</param>
    /// <param name="newValue">The new value.</param>
    /// <param name="oldValue">The old value.</param>
    /// <param name="description">The description of the operation.</param>
    /// <exception cref="ArgumentNullException"><paramref name="history"/> or <paramref name="setter"/> is null.</exception>
    public static void Execute<T>(
        this IHistory history,
        Action<T> setter,
        T? newValue,
        T? oldValue,
        object? description = null
    )
    {
        ArgumentNullException.ThrowIfNull(history);
        ArgumentNullException.ThrowIfNull(setter);

        history.Execute(new ValueUndo<T>(description, setter, newValue, oldValue));
    }
}
