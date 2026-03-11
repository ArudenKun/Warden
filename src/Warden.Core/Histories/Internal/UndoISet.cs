using Warden.Core.Histories.Extensions;

namespace Warden.Core.Histories.Internal;

internal sealed class UndoISet<T> : UndoICollection<T>, ISet<T>
{
    private readonly ISet<T> _source;

    public UndoISet(
        IUndoManager manager,
        ISet<T> source,
        Func<UndoCollectionOperation, object?>? descriptionFactory
    )
        : base(manager, source, descriptionFactory)
    {
        _source = source;
    }

    #region ISet

    bool ISet<T>.Add(T item) =>
        _manager.DoAdd(
            _source,
            item,
            _descriptionFactory?.Invoke(
                new UndoCollectionOperation(this, UndoCollectionAction.ISetAdd, item)
            )
        );

    void ISet<T>.ExceptWith(IEnumerable<T> other)
    {
        if (_source.Count > 0)
        {
            using IUndoTransaction transaction = _manager.BeginTransaction(
                _descriptionFactory?.Invoke(
                    new UndoCollectionOperation(this, UndoCollectionAction.ISetExceptWith, other)
                )
            );

            foreach (T item in other)
            {
                _manager.DoRemove(_source, item);
            }

            transaction.Commit();
        }
    }

    void ISet<T>.IntersectWith(IEnumerable<T> other)
    {
        if (_source.Count > 0)
        {
            List<T> items = [.. other.Where(_source.Contains)];

            using IUndoTransaction transaction = _manager.BeginTransaction(
                _descriptionFactory?.Invoke(
                    new UndoCollectionOperation(this, UndoCollectionAction.ISetIntersectWith, other)
                )
            );

            _manager.DoClear(_source);
            foreach (T item in items)
            {
                _manager.DoAdd(_source, item);
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
        using IUndoTransaction transaction = _manager.BeginTransaction(
            _descriptionFactory?.Invoke(
                new UndoCollectionOperation(
                    this,
                    UndoCollectionAction.ISetSymmetricExceptWith,
                    other
                )
            )
        );

        foreach (T item in other)
        {
            if (!_manager.DoRemove(_source, item))
            {
                _manager.DoAdd(_source, item);
            }
        }

        transaction.Commit();
    }

    void ISet<T>.UnionWith(IEnumerable<T> other)
    {
        using IUndoTransaction transaction = _manager.BeginTransaction(
            _descriptionFactory?.Invoke(
                new UndoCollectionOperation(this, UndoCollectionAction.ISetUnionWith, other)
            )
        );

        foreach (T item in other)
        {
            _manager.DoAdd(_source, item);
        }

        transaction.Commit();
    }

    #endregion
}
