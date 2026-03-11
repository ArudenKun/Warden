namespace Warden.Core.Histories.R3;

/// <summary>
/// Stack history extension methods for the generic list implementations.
/// </summary>
public static class ListExtensions
{
    /// <summary>
    /// Adds item to the source list with history.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="source">The source list.</param>
    /// <param name="item">The item to add.</param>
    /// <param name="history">The history object.</param>
    public static void AddWithHistory<T>(this IList<T> source, T item, IHistory history)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (item == null)
            throw new ArgumentNullException(nameof(item));
        ArgumentNullException.ThrowIfNull(history);

        int index = source.Count;
        history.Snapshot(Undo, Redo);
        Redo();
        return;
        void Undo() => source.RemoveAt(index);
        void Redo() => source.Insert(index, item);
    }

    /// <summary>
    /// Inserts item to the source list with history.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="source">The source list.</param>
    /// <param name="index">The item insertion index.</param>
    /// <param name="item">The item to insert.</param>
    /// <param name="history">The history object.</param>
    public static void InsertWithHistory<T>(
        this IList<T> source,
        int index,
        T item,
        IHistory history
    )
    {
        ArgumentNullException.ThrowIfNull(source);

        if (index < 0)
            throw new IndexOutOfRangeException("Index can not be negative.");

        if (item == null)
            throw new ArgumentNullException(nameof(item));

        ArgumentNullException.ThrowIfNull(history);

        history.Snapshot(Undo, Redo);
        Redo();
        return;

        void Redo() => source.Insert(index, item);
        void Undo() => source.RemoveAt(index);
    }

    /// <summary>
    /// Replaces item at specified index in the source list with history.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="source">The source list.</param>
    /// <param name="index">The item index to replace.</param>
    /// <param name="item">The replaced item.</param>
    /// <param name="history">The history object.</param>
    public static void ReplaceWithHistory<T>(
        this IList<T> source,
        int index,
        T item,
        IHistory history
    )
    {
        ArgumentNullException.ThrowIfNull(source);

        if (index < 0)
            throw new IndexOutOfRangeException("Index can not be negative.");

        if (item == null)
            throw new ArgumentNullException(nameof(item));

        ArgumentNullException.ThrowIfNull(history);

        var oldValue = source[index];
        var newValue = item;
        history.Snapshot(Undo, Redo);
        Redo();
        return;
        void Redo() => source[index] = newValue;
        void Undo() => source[index] = oldValue;
    }

    /// <summary>
    /// Removes item at specified index from the source list with history.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="source">The source list.</param>
    /// <param name="item">The item to remove.</param>
    /// <param name="history">The history object.</param>
    public static void RemoveWithHistory<T>(this IList<T> source, T item, IHistory history)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (item == null)
            throw new ArgumentNullException(nameof(item));

        ArgumentNullException.ThrowIfNull(history);

        int index = source.IndexOf(item);
        history.Snapshot(Undo, Redo);
        Redo();
        return;
        void Redo() => source.RemoveAt(index);
        void Undo() => source.Insert(index, item);
    }

    /// <summary>
    /// Removes item from the source list with history.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="source">The source list.</param>
    /// <param name="index">The item index to remove.</param>
    /// <param name="history">The history object.</param>
    public static void RemoveWithHistory<T>(this IList<T> source, int index, IHistory history)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (index < 0)
            throw new IndexOutOfRangeException("Index can not be negative.");

        ArgumentNullException.ThrowIfNull(history);

        var item = source[index];
        history.Snapshot(Undo, Redo);
        Redo();
        return;
        void Redo() => source.RemoveAt(index);
        void Undo() => source.Insert(index, item);
    }

    /// <summary>
    /// Removes all items from the source list with history.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="source">The source list.</param>
    /// <param name="history">The history object.</param>
    public static void ClearWithHistory<T>(this IList<T> source, IHistory history)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(history);

        if (source.Count > 0)
        {
            var items = source.ToArray();

            void Redo()
            {
                foreach (var item in items)
                {
                    source.Remove(item);
                }
            }

            void Undo()
            {
                foreach (var item in items)
                {
                    source.Add(item);
                }
            }

            history.Snapshot(Undo, Redo);
            Redo();
        }
    }
}
