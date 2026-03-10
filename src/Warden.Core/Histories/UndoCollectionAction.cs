namespace Warden.Core.Histories;

/// <summary>
/// Describes the action that will be performed on an <see cref="ICollection{T}"/> and recorded by an <see cref="IHistory"/>.
/// </summary>
public enum UndoCollectionAction
{
    /// <summary>
    /// <see cref="ICollection{T}.Add(T)"/> will be performed.
    /// </summary>
    CollectionAdd,

    /// <summary>
    /// <see cref="ICollection{T}.Remove(T)"/> will be performed.
    /// </summary>
    CollectionRemove,

    /// <summary>
    /// <see cref="ICollection{T}.Clear"/> will be performed.
    /// </summary>
    CollectionClear,

    /// <summary>
    /// <see cref="ISet{T}.Add(T)"/> will be performed.
    /// </summary>
    SetAdd,

    /// <summary>
    /// <see cref="ISet{T}.ExceptWith(IEnumerable{T})"/> will be performed.
    /// </summary>
    SetExceptWith,

    /// <summary>
    /// <see cref="ISet{T}.IntersectWith(IEnumerable{T})"/> will be performed.
    /// </summary>
    SetIntersectWith,

    /// <summary>
    /// <see cref="ISet{T}.SymmetricExceptWith(IEnumerable{T})"/> will be performed.
    /// </summary>
    SetSymmetricExceptWith,

    /// <summary>
    /// <see cref="ISet{T}.UnionWith(IEnumerable{T})"/> will be performed.
    /// </summary>
    SetUnionWith,

    /// <summary>
    /// <see cref="ListExtensions.Move{T}(IList{T}, int, int)"/> will be performed.
    /// </summary>
    ListMove,

    /// <summary>
    /// <see cref="IList{T}.Insert(int, T)"/> will be performed.
    /// </summary>
    ListInsert,

    /// <summary>
    /// <see cref="IList{T}.RemoveAt(int)"/> will be performed.
    /// </summary>
    ListRemoveAt,

    /// <summary>
    /// <see cref="P:System.Collections.Generic.IList`1.Item(System.Int32)"/> will be performed.
    /// </summary>
    ListIndexer,

    /// <summary>
    /// <see cref="IDictionary{TKey, TValue}.Add(TKey, TValue)"/> will be performed.
    /// </summary>
    DictionaryAdd,

    /// <summary>
    /// <see cref="IDictionary{TKey, TValue}.Remove(TKey)"/> will be performed.
    /// </summary>
    DictionaryRemove,

    /// <summary>
    /// <see cref="P:System.Collections.Generic.IDictionary`2.Item(``0)"/> will be performed.
    /// </summary>
    DictionaryIndexer,
}
