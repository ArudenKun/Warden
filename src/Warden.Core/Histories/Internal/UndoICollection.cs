using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using Warden.Core.Histories.Extensions;

namespace Warden.Core.Histories.Internal;

internal class UndoICollection<T> : ICollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
{
    private readonly ICollection<T> _source;

    protected readonly IUndoManager _manager;
    protected readonly Func<UndoCollectionOperation, object?>? _descriptionFactory;

    public UndoICollection(
        IUndoManager manager,
        ICollection<T> source,
        Func<UndoCollectionOperation, object?>? descriptionFactory
    )
    {
        _source = source;
        _manager = manager;
        _descriptionFactory = descriptionFactory;
    }

    #region ICollection

    void ICollection<T>.Add(T item) =>
        _manager.DoAdd(
            _source,
            item,
            _descriptionFactory?.Invoke(
                new UndoCollectionOperation(this, UndoCollectionAction.ICollectionAdd, item)
            )
        );

    void ICollection<T>.Clear() =>
        _manager.DoClear(
            _source,
            _descriptionFactory?.Invoke(
                new UndoCollectionOperation(this, UndoCollectionAction.ICollectionClear)
            )
        );

    bool ICollection<T>.Contains(T item) => _source.Contains(item);

    void ICollection<T>.CopyTo(T[] array, int arrayIndex) => _source.CopyTo(array, arrayIndex);

    int ICollection<T>.Count => _source.Count;

    bool ICollection<T>.IsReadOnly => _source.IsReadOnly;

    bool ICollection<T>.Remove(T item) =>
        _manager.DoRemove(
            _source,
            item,
            _descriptionFactory?.Invoke(
                new UndoCollectionOperation(this, UndoCollectionAction.ICollectionRemove, item)
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
