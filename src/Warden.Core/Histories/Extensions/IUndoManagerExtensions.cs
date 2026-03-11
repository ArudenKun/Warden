namespace Warden.Core.Histories.Extensions;

/// <summary>
/// Provides methods to create <see cref="IUndo"/> command and add them to an <see cref="IUndoManager"/>.
/// </summary>
public static class IUndoManagerExtensions
{
    /// <summary>
    /// Adds an item from a <see cref="ISet{T}"/> as a <see cref="IUndo"/> operation.
    /// </summary>
    /// <typeparam name="T">The type of element in the <see cref="ISet{T}"/>.</typeparam>
    /// <param name="manager">The <see cref="IUndoManager"/>.</param>
    /// <param name="source">The <see cref="ISet{T}"/>.</param>
    /// <param name="item">The item to add.</param>
    /// <param name="description">The description of the operation.</param>
    /// <returns>true if the command has been created, false if not because <paramref name="source"/> already contained <paramref name="item"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="manager"/> or <paramref name="source"/> is null.</exception>
    public static bool DoAdd<T>(
        this IUndoManager manager,
        ISet<T> source,
        T? item,
        object? description = null
    )
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(source);

        bool result = !source.Contains(item!);

        if (result)
        {
            manager.Do(new CollectionUndo<T>(description, source, item, true));
        }

        return result;
    }

    /// <summary>
    /// Adds a value to a <see cref="ICollection{T}"/> as a <see cref="IUndo"/> operation.
    /// </summary>
    /// <typeparam name="T">The type of element in the <see cref="ICollection{T}"/>.</typeparam>
    /// <param name="manager">The <see cref="IUndoManager"/>.</param>
    /// <param name="source">The <see cref="ICollection{T}"/>.</param>
    /// <param name="item">The item to add.</param>
    /// <param name="description">The description of the operation.</param>
    /// <exception cref="ArgumentNullException"><paramref name="manager"/> or <paramref name="source"/> is null.</exception>
    public static void DoAdd<T>(
        this IUndoManager manager,
        ICollection<T> source,
        T? item,
        object? description = null
    )
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(source);

        if (source is IList<T> list)
        {
            manager.DoInsert(list!, list.Count, item, description);
        }
        else
        {
            manager.Do(new CollectionUndo<T>(description, source, item, true));
        }
    }

    /// <summary>
    /// Clears a <see cref="ICollection{T}"/> as a <see cref="IUndo"/> operation.
    /// </summary>
    /// <typeparam name="T">The type of element in the <see cref="ICollection{T}"/>.</typeparam>
    /// <param name="manager">The <see cref="IUndoManager"/>.</param>
    /// <param name="source">The <see cref="ICollection{T}"/>.</param>
    /// <param name="description">The description of the operation.</param>
    /// <exception cref="ArgumentNullException"><paramref name="manager"/> or <paramref name="source"/> is null.</exception>
    public static void DoClear<T>(
        this IUndoManager manager,
        ICollection<T> source,
        object? description = null
    )
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(source);

        if (source.Count > 0)
        {
            T[] oldValues = [.. source];

            manager.Do(
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
    /// <param name="manager">The <see cref="IUndoManager"/>.</param>
    /// <param name="source">The <see cref="ICollection{T}"/>.</param>
    /// <param name="item">The item to remove.</param>
    /// <param name="description">The description of the operation.</param>
    /// <returns>true if the command has been created, false if not because <paramref name="source"/> did not contained <paramref name="item"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="manager"/> or <paramref name="source"/> is null.</exception>
    public static bool DoRemove<T>(
        this IUndoManager manager,
        ICollection<T> source,
        T? item,
        object? description = null
    )
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(source);

        bool result = false;

        if (source is IList<T> list)
        {
            int index = list.IndexOf(item!);
            if (index >= 0)
            {
                manager.DoRemoveAt(list, index, description);
                result = true;
            }
        }
        else if (source.Contains(item!))
        {
            manager.Do(new CollectionUndo<T>(description, source, item, false));
            result = true;
        }

        return result;
    }

    /// <summary>
    /// Adds a value to a <see cref="IDictionary{TKey, TValue}"/> as a <see cref="IUndo"/> operation.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="manager">The <see cref="IUndoManager"/>.</param>
    /// <param name="source">The <see cref="IDictionary{TKey, TValue}"/>.</param>
    /// <param name="key">The key to add.</param>
    /// <param name="value">The value to add.</param>
    /// <param name="description">The description of the operation.</param>
    /// <exception cref="ArgumentNullException"><paramref name="manager"/>, <paramref name="source"/> or <paramref name="key"/> is null.</exception>
    public static void DoAdd<TKey, TValue>(
        this IUndoManager manager,
        IDictionary<TKey, TValue> source,
        TKey key,
        TValue? value,
        object? description = null
    )
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(key);

        manager.Do(new DictionaryUndo<TKey, TValue>(description, source, key, value, true));
    }

    /// <summary>
    /// Removes the item with the specified key from a <see cref="IDictionary{TKey, TValue}"/> as a <see cref="IUndo"/> operation.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="manager">The <see cref="IUndoManager"/>.</param>
    /// <param name="source">The <see cref="IDictionary{TKey, TValue}"/>.</param>
    /// <param name="key">The key to remove.</param>
    /// <param name="description">The description of the operation.</param>
    /// <returns>true if the command has been created, false if not because <paramref name="source"/> did not contained <paramref name="key"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="manager"/>, <paramref name="source"/> or <paramref name="key"/> is null.</exception>
    public static bool DoRemove<TKey, TValue>(
        this IUndoManager manager,
        IDictionary<TKey, TValue> source,
        TKey key,
        object? description = null
    )
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(key);

        bool result = false;
        if (source.TryGetValue(key, out TValue? value))
        {
            manager.Do(new DictionaryUndo<TKey, TValue>(description, source, key, value, false));
            result = true;
        }

        return result;
    }

    /// <summary>
    /// Sets the element with the specified key on a <see cref="IDictionary{TKey, TValue}"/> as a <see cref="IUndo"/> operation.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="manager">The <see cref="IUndoManager"/>.</param>
    /// <param name="source">The <see cref="IDictionary{TKey, TValue}"/>.</param>
    /// <param name="key">The key to set.</param>
    /// <param name="value">The value to add.</param>
    /// <param name="description">The description of the operation.</param>
    /// <exception cref="ArgumentNullException"><paramref name="manager"/>, <paramref name="source"/> or <paramref name="key"/> is null.</exception>
    public static void Do<TKey, TValue>(
        this IUndoManager manager,
        IDictionary<TKey, TValue> source,
        TKey key,
        TValue? value,
        object? description = null
    )
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(key);

        if (source.TryGetValue(key, out TValue? oldValue))
        {
            manager.Do(v => source[key] = v!, value, oldValue, description);
        }
        else
        {
            manager.Do(() => source[key] = value!, () => source.Remove(key), description);
        }
    }

    /// <summary>
    /// Inserts an item to a <see cref="IList{T}"/> at the specified index as a <see cref="IUndo"/> operation.
    /// </summary>
    /// <typeparam name="T">The type of element in the <see cref="IList{T}"/>.</typeparam>
    /// <param name="manager">The <see cref="IUndoManager"/>.</param>
    /// <param name="source">The <see cref="IList{T}"/>.</param>
    /// <param name="index">The zero-based index at which item should be inserted.</param>
    /// <param name="item">The item to insert into the <see cref="IList{T}"/>.</param>
    /// <param name="description">The description of the operation.</param>
    /// <exception cref="ArgumentNullException"><paramref name="manager"/> or <paramref name="source"/> is null.</exception>
    public static void DoInsert<T>(
        this IUndoManager manager,
        IList<T> source,
        int index,
        T? item,
        object? description = null
    )
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(source);

        manager.Do(new ListUndo<T>(description, source, index, item, true));
    }

    /// <summary>
    /// Removes an item from a <see cref="IList{T}"/> at the specified index as a <see cref="IUndo"/> operation.
    /// </summary>
    /// <typeparam name="T">The type of element in the <see cref="IList{T}"/>.</typeparam>
    /// <param name="manager">The <see cref="IUndoManager"/>.</param>
    /// <param name="source">The <see cref="IList{T}"/>.</param>
    /// <param name="index">The zero-based index at which item should be removed.</param>
    /// <param name="description">The description of the operation.</param>
    /// <exception cref="ArgumentNullException"><paramref name="manager"/> or <paramref name="source"/> is null.</exception>
    public static void DoRemoveAt<T>(
        this IUndoManager manager,
        IList<T> source,
        int index,
        object? description = null
    )
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(source);

        manager.Do(new ListUndo<T>(description, source, index, source[index], false));
    }

    /// <summary>
    /// Sets the element at the specified index on a <see cref="IList{T}"/> as a <see cref="IUndo"/> operation.
    /// </summary>
    /// <typeparam name="T">The type of element in the <see cref="IList{T}"/>.</typeparam>
    /// <param name="manager">The <see cref="IUndoManager"/>.</param>
    /// <param name="source">The <see cref="IList{T}"/>.</param>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <param name="item">The new item.</param>
    /// <param name="description">The description of the operation.</param>
    /// <exception cref="ArgumentNullException"><paramref name="manager"/> or <paramref name="source"/> is null.</exception>
    public static void Do<T>(
        this IUndoManager manager,
        IList<T> source,
        int index,
        T? item,
        object? description = null
    )
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(source);

        manager.Do(v => source[index] = v!, item, source[index], description);
    }

    /// <summary>
    /// Does a <see cref="IUndo"/> operation on the manager with the specified doAction and undoAction.
    /// </summary>
    /// <param name="manager">The <see cref="IUndoManager"/>.</param>
    /// <param name="doAction">The <see cref="Action"/> performed by <see cref="IUndo.Do"/>.</param>
    /// <param name="undoAction">The <see cref="Action"/> performed by the <see cref="IUndo.Undo"/>.</param>
    /// <param name="description">The description of the operation.</param>
    /// <exception cref="ArgumentNullException"><paramref name="manager"/> is null.</exception>
    public static void Do(
        this IUndoManager manager,
        Action? doAction,
        Action? undoAction,
        object? description = null
    )
    {
        ArgumentNullException.ThrowIfNull(manager);

        if (doAction != null || undoAction != null)
        {
            manager.Do(new Undo(description, doAction, undoAction));
        }
    }

    /// <summary>
    /// Does a <see cref="IUndo"/> operation on the manager with the specified action with no undo.
    /// </summary>
    /// <param name="manager">The <see cref="IUndoManager"/>.</param>
    /// <param name="action">The <see cref="Action"/> performed by <see cref="IUndo.Do"/>.</param>
    /// <param name="description">The description of the operation.</param>
    /// <exception cref="ArgumentNullException"><paramref name="manager"/> is null.</exception>
    public static void DoOnDo(
        this IUndoManager manager,
        Action action,
        object? description = null
    ) => manager.Do(action, null, description);

    /// <summary>
    /// Does a <see cref="IUndo"/> operation on the manager with the specified action with no do.
    /// </summary>
    /// <param name="manager">The <see cref="IUndoManager"/>.</param>
    /// <param name="action">The <see cref="Action"/> performed by <see cref="IUndo.Undo"/>.</param>
    /// <param name="description">The description of the operation.</param>
    /// <exception cref="ArgumentNullException"><paramref name="manager"/> is null.</exception>
    public static void DoOnUndo(
        this IUndoManager manager,
        Action action,
        object? description = null
    ) => manager.Do(null, action, description);

    /// <summary>
    /// Sets a value as a <see cref="IUndo"/> operation.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="manager">The <see cref="IUndoManager"/>.</param>
    /// <param name="setter">The <see cref="Action{T}"/> used to change the value.</param>
    /// <param name="newValue">The new value.</param>
    /// <param name="oldValue">The old value.</param>
    /// <param name="description">The description of the operation.</param>
    /// <exception cref="ArgumentNullException"><paramref name="manager"/> or <paramref name="setter"/> is null.</exception>
    public static void Do<T>(
        this IUndoManager manager,
        Action<T> setter,
        T? newValue,
        T? oldValue,
        object? description = null
    )
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(setter);

        manager.Do(new ValueUndo<T>(description, setter, newValue, oldValue));
    }
}
