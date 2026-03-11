using System.Diagnostics.CodeAnalysis;

namespace Warden.Core.Histories;

/// <summary>
/// Provides a global <see cref="TimeSpan"/> to use as default merge interval for all <see cref="ValueUndo{T}"/>.
/// </summary>
public static class ValueUnDo
{
    /// <summary>
    /// Represents a method that will be called when merging two <see cref="ValueUndo{T}"/> instances to get the resulting description.
    /// </summary>
    /// <param name="oldDescription">The description of the previous <see cref="ValueUndo{T}"/> merged.</param>
    /// <param name="newDescription">The description of the new <see cref="ValueUndo{T}"/> merged.</param>
    public delegate object? MergeDescriptionHandler(object? oldDescription, object? newDescription);

    /// <summary>
    /// The <see cref="TimeSpan"/> interval equivalent <see cref="ValueUndo{T}"/> instances should respect to be mergeable.
    /// Default value is 500ms.
    /// </summary>
    public static TimeSpan MergeInterval { get; set; } = TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// The <see cref="MergeDescriptionHandler"/> used to merge description between two <see cref="ValueUndo{T}"/> instance.
    /// </summary>
    public static MergeDescriptionHandler? MergeDescriptionAction { get; set; }
}

/// <summary>
/// Provides an implementation of the <see cref="IUndo"/> interface for setting value.
/// </summary>
/// <typeparam name="T">The type of value.</typeparam>
public sealed class ValueUndo<T> : IMergeableUndo
{
    /// <summary>
    /// Represents a method that will be called when merging two <see cref="ValueUndo{T}"/> instances to get the resulting description.
    /// </summary>
    /// <param name="oldDescription">The description of the previous <see cref="ValueUndo{T}"/> merged.</param>
    /// <param name="oldValue">The old value used when undoing the resulting merged <see cref="ValueUndo{T}"/>.</param>
    /// <param name="newDescription">The description of the new <see cref="ValueUndo{T}"/> merged.</param>
    /// <param name="newValue">The new value used when redoing the resulting merged <see cref="ValueUndo{T}"/>.</param>
    /// <returns>The new description that will be using for the resulting merged <see cref="ValueUndo{T}"/>.</returns>
    public delegate object MergeDescriptionHandler(
        object? oldDescription,
        T oldValue,
        object? newDescription,
        T newValue
    );

    private readonly DateTime _timeStamp;
    private readonly object? _description;
    private readonly Action<T> _setter;

    [AllowNull]
    private readonly T _newValue;

    [AllowNull]
    private readonly T _oldValue;

    /// <summary>
    /// The <see cref="TimeSpan"/> interval equivalent <see cref="ValueUndo{T}"/> instances should respect to be mergeable.
    /// If not set, <see cref="ValueUnDo.MergeInterval"/> will be used.
    /// </summary>
    public static TimeSpan? MergeInterval { get; set; }

    /// <summary>
    /// The <see cref="MergeDescriptionHandler"/> used to merge description between two <see cref="ValueUndo{T}"/> instance.
    /// </summary>
    public static MergeDescriptionHandler? MergeDescriptionAction { get; set; }

    /// <summary>
    /// Initialises an instance of <see cref="ValueUndo{T}"/>.
    /// </summary>
    /// <param name="description">The description of this <see cref="IUndo"/></param>
    /// <param name="setter">The action called to set the value.</param>
    /// <param name="newValue">The new value.</param>
    /// <param name="oldValue">The old value.</param>
    /// <exception cref="ArgumentNullException"><paramref name="setter"/> is null.</exception>
    public ValueUndo(
        object? description,
        Action<T> setter,
        [AllowNull] T newValue,
        [AllowNull] T oldValue
    )
    {
        ArgumentNullException.ThrowIfNull(setter);

        _timeStamp = DateTime.Now;
        _description = description;
        _setter = setter;
        _newValue = newValue;
        _oldValue = oldValue;
    }

    /// <summary>
    /// Initialises an instance of <see cref="ValueUndo{T}"/>.
    /// </summary>
    /// <param name="setter">The action called to set the value.</param>
    /// <param name="newValue">The new value.</param>
    /// <param name="oldValue">The old value.</param>
    /// <exception cref="ArgumentNullException"><paramref name="setter"/> is null.</exception>
    public ValueUndo(Action<T> setter, T newValue, T oldValue)
        : this(null, setter, newValue, oldValue) { }

    #region IMergeableUnDo

    /// <inheritdoc />
    bool IMergeableUndo.TryMerge(IUndo other, [NotNullWhen(true)] out IUndo? mergedCommand)
    {
        mergedCommand =
            (
                other is ValueUndo<T> value
                || (other is GroupUndo group && group.TryGetSingle(out value))
            )
            && _setter == value._setter
            && Equals(_newValue, value._oldValue)
            && (value._timeStamp - _timeStamp) < (MergeInterval ?? ValueUnDo.MergeInterval)
                ? new ValueUndo<T>(
                    MergeDescriptionAction?.Invoke(
                        _description,
                        _oldValue,
                        value._description,
                        value._newValue
                    )
                        ?? ValueUnDo.MergeDescriptionAction?.Invoke(
                            _description,
                            value._description
                        )
                        ?? value._description,
                    _setter,
                    value._newValue,
                    _oldValue
                )
                : null;

        return mergedCommand != null;
    }

    #endregion

    #region IUnDo

    /// <inheritdoc />
    object? IUndo.Description => _description;

    /// <inheritdoc />
    void IUndo.Do() => _setter.Invoke(_newValue);

    /// <inheritdoc />
    void IUndo.Undo() => _setter.Invoke(_oldValue);

    #endregion
}
