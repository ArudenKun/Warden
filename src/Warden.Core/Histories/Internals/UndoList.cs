using System.Collections.ObjectModel;

namespace Warden.Core.Histories.Internals;

internal sealed class UndoList<T> : UndoCollection<T>, IList<T>
{
    private readonly IList<T> _source;

    public UndoList(
        IHistory manager,
        IList<T> source,
        Func<UnDoCollectionOperation, object?>? descriptionFactory
    )
        : base(manager, source, descriptionFactory)
    {
        _source = source;
    }

    public void Move(int oldIndex, int newIndex)
    {
        if (_source is ObservableCollection<T> collection)
        {
            History.Execute(
                () => collection.Move(oldIndex, newIndex),
                () => collection.Move(newIndex, oldIndex),
                DescriptionFactory?.Invoke(
                    new UnDoCollectionOperation(
                        this,
                        UndoCollectionAction.ListMove,
                        oldIndex,
                        newIndex
                    )
                )
            );
        }
        else
        {
            using IUndoTransaction transaction = History.BeginTransaction(
                DescriptionFactory?.Invoke(
                    new UnDoCollectionOperation(
                        this,
                        UndoCollectionAction.ListMove,
                        oldIndex,
                        newIndex
                    )
                )
            );

            T item = _source[oldIndex];
            IList<T> list = this;
            list.RemoveAt(oldIndex);
            list.Insert(newIndex, item);

            transaction.Commit();
        }
    }

    #region IList

    int IList<T>.IndexOf(T item) => _source.IndexOf(item);

    void IList<T>.Insert(int index, T item) =>
        History.ExecuteInsert(
            _source,
            index,
            item,
            DescriptionFactory?.Invoke(
                new UnDoCollectionOperation(this, UndoCollectionAction.ListInsert, index, item)
            )
        );

    void IList<T>.RemoveAt(int index) =>
        History.ExecuteRemoveAt(
            _source,
            index,
            DescriptionFactory?.Invoke(
                new UnDoCollectionOperation(this, UndoCollectionAction.ListRemoveAt, index)
            )
        );

    T IList<T>.this[int index]
    {
        get => _source[index];
        set =>
            History.Execute(
                _source,
                index,
                value,
                DescriptionFactory?.Invoke(
                    new UnDoCollectionOperation(
                        this,
                        UndoCollectionAction.ListIndexer,
                        index,
                        value
                    )
                )
            );
    }

    #endregion
}
