using System.Diagnostics.CodeAnalysis;

namespace Warden.Core.Histories.Internals;

internal sealed class UndoDictionary<TKey, TValue>
    : UndoCollection<KeyValuePair<TKey, TValue>>,
        IDictionary<TKey, TValue>
{
    private readonly IDictionary<TKey, TValue> _source;

    public UndoDictionary(
        IHistory history,
        IDictionary<TKey, TValue> source,
        Func<UnDoCollectionOperation, object?>? descriptionFactory
    )
        : base(history, source, descriptionFactory)
    {
        _source = source;
    }

    #region IDictionary

    void IDictionary<TKey, TValue>.Add(TKey key, TValue value) =>
        History.ExecuteAdd(
            _source,
            key,
            value,
            DescriptionFactory?.Invoke(
                new UnDoCollectionOperation(this, UndoCollectionAction.DictionaryAdd, key, value)
            )
        );

    bool IDictionary<TKey, TValue>.ContainsKey(TKey key) => _source.ContainsKey(key);

    bool IDictionary<TKey, TValue>.Remove(TKey key) =>
        History.ExecuteRemove(
            _source,
            key,
            DescriptionFactory?.Invoke(
                new UnDoCollectionOperation(this, UndoCollectionAction.DictionaryRemove, key)
            )
        );

    bool IDictionary<TKey, TValue>.TryGetValue(TKey key,
#if NET5_0_OR_GREATER
        [MaybeNullWhen(false)]
#endif
        out TValue value) => _source.TryGetValue(key, out value);

    TValue IDictionary<TKey, TValue>.this[TKey key]
    {
        get => _source[key];
        set =>
            History.Execute(
                _source,
                key,
                value,
                DescriptionFactory?.Invoke(
                    new UnDoCollectionOperation(
                        this,
                        UndoCollectionAction.DictionaryIndexer,
                        key,
                        value
                    )
                )
            );
    }

    ICollection<TKey> IDictionary<TKey, TValue>.Keys => _source.Keys;

    ICollection<TValue> IDictionary<TKey, TValue>.Values => _source.Values;

    #endregion
}
