namespace Warden.Core.Histories.Internals;

internal sealed class UndoSet<T> : UndoCollection<T>, ISet<T>
{
    private readonly ISet<T> _source;

    public UndoSet(
        IHistory manager,
        ISet<T> source,
        Func<UnDoCollectionOperation, object?>? descriptionFactory
    )
        : base(manager, source, descriptionFactory)
    {
        _source = source;
    }

    #region ISet

    bool ISet<T>.Add(T item) =>
        History.ExecuteAdd(
            _source,
            item,
            DescriptionFactory?.Invoke(
                new UnDoCollectionOperation(this, UndoCollectionAction.SetAdd, item)
            )
        );

    void ISet<T>.ExceptWith(IEnumerable<T> other)
    {
        if (_source.Count > 0)
        {
            using IUndoTransaction transaction = History.BeginTransaction(
                DescriptionFactory?.Invoke(
                    new UnDoCollectionOperation(this, UndoCollectionAction.SetExceptWith, other)
                )
            );

            foreach (T item in other)
            {
                History.ExecuteRemove(_source, item);
            }

            transaction.Commit();
        }
    }

    void ISet<T>.IntersectWith(IEnumerable<T> other)
    {
        if (_source.Count > 0)
        {
            List<T> items = [.. other.Where(_source.Contains)];

            using IUndoTransaction transaction = History.BeginTransaction(
                DescriptionFactory?.Invoke(
                    new UnDoCollectionOperation(this, UndoCollectionAction.SetIntersectWith, other)
                )
            );

            History.ExecuteClear(_source);
            foreach (T item in items)
            {
                History.ExecuteAdd(_source, item);
            }

            transaction.Commit();
        }
    }

    bool ISet<T>.IsProperSubsetOf(IEnumerable<T> other) => _source.IsProperSubsetOf(other);

    bool ISet<T>.IsProperSupersetOf(IEnumerable<T> other) => _source.IsProperSupersetOf(other);

    bool ISet<T>.IsSubsetOf(IEnumerable<T> other) => _source.IsSubsetOf(other);

    bool ISet<T>.IsSupersetOf(IEnumerable<T> other) => _source.IsSupersetOf(other);

    bool ISet<T>.Overlaps(IEnumerable<T> other) => _source.Overlaps(other);

    bool ISet<T>.SetEquals(IEnumerable<T> other) => _source.SetEquals(other);

    void ISet<T>.SymmetricExceptWith(IEnumerable<T> other)
    {
        using IUndoTransaction transaction = History.BeginTransaction(
            DescriptionFactory?.Invoke(
                new UnDoCollectionOperation(
                    this,
                    UndoCollectionAction.SetSymmetricExceptWith,
                    other
                )
            )
        );

        foreach (T item in other)
        {
            if (!History.ExecuteRemove(_source, item))
            {
                History.ExecuteAdd(_source, item);
            }
        }

        transaction.Commit();
    }

    void ISet<T>.UnionWith(IEnumerable<T> other)
    {
        using IUndoTransaction transaction = History.BeginTransaction(
            DescriptionFactory?.Invoke(
                new UnDoCollectionOperation(this, UndoCollectionAction.SetUnionWith, other)
            )
        );

        foreach (T item in other)
        {
            History.ExecuteAdd(_source, item);
        }

        transaction.Commit();
    }

    #endregion
}
