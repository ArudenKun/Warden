namespace Warden.Core.Histories.Actions;

/// <summary>
/// Represents an undoable property change action.
/// </summary>
/// <typeparam name="TTarget">The type of the target object.</typeparam>
/// <typeparam name="TValue">The type of the property value.</typeparam>
public class PropertyChangeAction<TTarget, TValue> : IHistoryAction
{
    private readonly TTarget _target;
    private readonly Action<TTarget, TValue> _setter;
    private readonly TValue _oldValue;
    private readonly TValue _newValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyChangeAction{TTarget, TValue}"/> class.
    /// </summary>
    /// <param name="target">The target object whose property is being changed.</param>
    /// <param name="setter">The action to set the property value.</param>
    /// <param name="oldValue">The old value of the property.</param>
    /// <param name="newValue">The new value of the property.</param>
    /// <param name="description">The description of the action.</param>
    public PropertyChangeAction(
        TTarget target,
        Action<TTarget, TValue> setter,
        TValue oldValue,
        TValue newValue,
        string? description = null
    )
    {
        _target = target ?? throw new ArgumentNullException(nameof(target));
        _setter = setter ?? throw new ArgumentNullException(nameof(setter));
        _oldValue = oldValue;
        _newValue = newValue;
        Description = description ?? $"Property change on {typeof(TTarget).Name}";
    }

    /// <summary>
    /// Gets the description of the action.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Undoes the property change.
    /// </summary>
    public void Undo()
    {
        _setter(_target, _oldValue);
    }

    /// <summary>
    /// Redoes the property change.
    /// </summary>
    public void Redo()
    {
        _setter(_target, _newValue);
    }
}
