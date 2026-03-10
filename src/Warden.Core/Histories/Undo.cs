namespace Warden.Core.Histories;

/// <summary>
/// Provides an implementation of the <see cref="IUndo"/> interface for custom do and undo action.
/// </summary>
public sealed class Undo : IUndo
{
    private readonly object? _description;
    private readonly Action? _doAction;
    private readonly Action? _undoAction;

    /// <summary>
    /// Initializes an instance of <see cref="Undo"/>.
    /// </summary>
    /// <param name="description">The description of this <see cref="IUndo"/></param>
    /// <param name="doAction">The action called by <see cref="IUndo.Execute"/>.</param>
    /// <param name="undoAction">The action called by <see cref="IUndo.Undo"/>.</param>
    public Undo(object? description, Action? doAction, Action? undoAction)
    {
        _description = description;
        _doAction = doAction;
        _undoAction = undoAction;
    }

    /// <summary>
    /// Initializes an instance of <see cref="Undo"/>.
    /// </summary>
    /// <param name="doAction">The action called by <see cref="IUndo.Execute"/>.</param>
    /// <param name="undoAction">The action called by <see cref="IUndo.Undo"/>.</param>
    public Undo(Action? doAction, Action? undoAction)
        : this(null, doAction, undoAction) { }

    #region IUndo

    /// <inheritdoc />
    object? IUndo.Description => _description;

    /// <inheritdoc />
    void IUndo.Execute() => _doAction?.Invoke();

    /// <inheritdoc />
    void IUndo.Undo() => _undoAction?.Invoke();

    #endregion
}
