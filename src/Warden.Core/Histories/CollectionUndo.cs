using System.Diagnostics.CodeAnalysis;

namespace Warden.Core.Histories;

/// <summary>
/// Provides an implementation of the <see cref="IUndo"/> interface for <see cref="ICollection{T}"/> operation.
/// </summary>
/// <typeparam name="T">The type of element in the <see cref="ICollection{T}"/>.</typeparam>
public sealed class CollectionUndo<T> : IUndo
{
    private readonly object? _description;
    private readonly ICollection<T> _source;

    [AllowNull]
    private readonly T _item;
    private readonly bool _isAdd;

    /// <summary>
    /// Initialise an instance of <see cref="CollectionUndo{T}"/>.
    /// </summary>
    /// <param name="description">The description of this <see cref="IUndo"/></param>
    /// <param name="source">The <see cref="ICollection{T}"/> on which to perform operation.</param>
    /// <param name="item">The argument of the operation.</param>
    /// <param name="isAdd">true if the operation is an <see cref="ICollection{T}.Add(T)"/>, false for a <see cref="ICollection{T}.Remove(T)"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
    public CollectionUndo(
        object? description,
        ICollection<T> source,
        [AllowNull] T item,
        bool isAdd
    )
    {
        ArgumentNullException.ThrowIfNull(source);

        _description = description;
        _source = source;
        _item = item;
        _isAdd = isAdd;
    }

    /// <summary>
    /// Initialise an instance of <see cref="CollectionUndo{T}"/>.
    /// </summary>
    /// <param name="source">The <see cref="ICollection{T}"/> on which to perform operation.</param>
    /// <param name="item">The argument of the operation.</param>
    /// <param name="isAdd">true if the operation is an <see cref="ICollection{T}.Add(T)"/>, false for a <see cref="ICollection{T}.Remove(T)"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
    public CollectionUndo(ICollection<T> source, [AllowNull] T item, bool isAdd)
        : this(null, source, item, isAdd) { }

    private void Action(bool isAdd)
    {
        if (isAdd)
        {
            _source.Add(_item);
        }
        else
        {
            _source.Remove(_item);
        }
    }

    #region IUndo

    /// <inheritdoc />
    object? IUndo.Description => _description;

    /// <inheritdoc />
    void IUndo.Undo() => Action(!_isAdd);

    /// <inheritdoc />
    void IUndo.Execute() => Action(_isAdd);

    #endregion
}
