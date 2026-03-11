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
    /// Does an action.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Naming",
        "CA1716:Identifiers should not match keywords",
        Justification = "we do"
    )]
    void Do();

    /// <summary>
    /// Does the opposite of the <see cref="Do"/> action.
    /// </summary>
    void Undo();
}
