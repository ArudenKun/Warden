using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Warden.Core.Histories.Internals;

internal class UndoCollection<T> : ICollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
{
    private readonly ICollection<T> _source;

    public UndoCollection(
        IHistory history,
        ICollection<T> source,
        Func<UnDoCollectionOperation, object?>? descriptionFactory
    )
    {
        _source = source;
        History = history;
        DescriptionFactory = descriptionFactory;
    }

    protected IHistory History { get; }
    protected Func<UnDoCollectionOperation, object?>? DescriptionFactory { get; }

    #region ICollection

    void ICollection<T>.Add(T item) =>
        History.ExecuteAdd(
            _source,
            item,
            DescriptionFactory?.Invoke(
                new UnDoCollectionOperation(this, UndoCollectionAction.CollectionAdd, item)
            )
        );

    void ICollection<T>.Clear() =>
        History.ExecuteClear(
            _source,
            DescriptionFactory?.Invoke(
                new UnDoCollectionOperation(this, UndoCollectionAction.CollectionClear)
            )
        );

    bool ICollection<T>.Contains(T item) => _source.Contains(item);

    void ICollection<T>.CopyTo(T[] array, int arrayIndex) => _source.CopyTo(array, arrayIndex);

    int ICollection<T>.Count => _source.Count;

    bool ICollection<T>.IsReadOnly => _source.IsReadOnly;

    bool ICollection<T>.Remove(T item) =>
        History.ExecuteRemove(
            _source,
            item,
            DescriptionFactory?.Invoke(
                new UnDoCollectionOperation(this, UndoCollectionAction.CollectionRemove, item)
            )
        );

    #endregion

    #region IEnumerator

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => _source.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_source).GetEnumerator();

    #endregion

    #region INotifyCollectionChanged

    event NotifyCollectionChangedEventHandler? INotifyCollectionChanged.CollectionChanged
    {
        add
        {
            if (_source is INotifyCollectionChanged collection)
            {
                collection.CollectionChanged += value;
            }
        }
        remove
        {
            if (_source is INotifyCollectionChanged collection)
            {
                collection.CollectionChanged -= value;
            }
        }
    }

    #endregion

    #region INotifyPropertyChanged

    event PropertyChangedEventHandler? INotifyPropertyChanged.PropertyChanged
    {
        add
        {
            if (_source is INotifyPropertyChanged collection)
            {
                collection.PropertyChanged += value;
            }
        }
        remove
        {
            if (_source is INotifyPropertyChanged collection)
            {
                collection.PropertyChanged -= value;
            }
        }
    }

    #endregion
}
