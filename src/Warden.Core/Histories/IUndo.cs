namespace Warden.Core.Histories;

/// <summary>
/// Provides methods to execute an action and remove its effect.
/// </summary>
public interface IUndo
{
    /// <summary>
    /// Gets a description of what this <see cref="IUndo"/> perform.
    /// </summary>
    object? Description { get; }

    /// <summary>
    /// Execute an action.
    /// </summary>
    void Execute();

    /// <summary>
    /// Does the opposite of the <see cref="Execute"/> action.
    /// </summary>
    void Undo();
}
