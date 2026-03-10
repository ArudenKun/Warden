using System.ComponentModel;

namespace Warden.Core.Histories;

public interface IHistory : INotifyPropertyChanged
{
    /// <summary>
    /// Gets an <see cref="int"/> representing the state of the <see cref="IHistory"/>.
    /// </summary>
    int Version { get; }

    /// <summary>
    /// Returns a boolean to express if the method <see cref="Undo"/> can be executed.
    /// </summary>
    /// <returns>true if <see cref="Undo"/> can be executed, else false.</returns>
    bool CanUndo { get; }

    /// <summary>
    /// Returns a boolean to express if the method <see cref="Redo"/> can be executed.
    /// </summary>
    /// <returns>true if <see cref="Redo"/> can be executed, else false.</returns>
    bool CanRedo { get; }

    /// <summary>
    /// Gets the descriptions in order of all the <see cref="IUndo"/> which can be undone.
    /// </summary>
    IEnumerable<object?> UndoDescriptions { get; }

    /// <summary>
    /// Gets the descriptions in order of all the <see cref="IUndo"/> which can be redone.
    /// </summary>
    IEnumerable<object?> RedoDescriptions { get; }

    /// <summary>
    /// Starts a group of operation and return an <see cref="IUndoTransaction"/> to stop the group.
    /// If <see cref="IUndoTransaction.Commit"/> is not called, all operations done during the transaction will be undone on <see cref="IDisposable.Dispose"/>.
    /// </summary>
    /// <param name="description">The description of the group of operations.</param>
    /// <returns>An <see cref="IUndoTransaction"/> to commit or rollback the transaction of operations.</returns>
    IUndoTransaction BeginTransaction(object? description = null);

    /// <summary>
    /// Clears the history of <see cref="IUndo"/> operations.
    /// </summary>
    void Clear();

    /// <summary>
    /// Executes the <see cref="IUndo"/> command and stores it in the manager history.
    /// </summary>
    /// <param name="command">The <see cref="IUndo"/> to execute.</param>
    /// <exception cref="ArgumentNullException"><paramref name="command"/> is null.</exception>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Naming",
        "CA1716:Identifiers should not match keywords",
        Justification = "we do"
    )]
    void Execute(IUndo command);

    /// <summary>
    /// Redoes the last undone <see cref="IUndo"/> command of the manager history.
    /// </summary>
    /// <exception cref="InvalidOperationException">Cannot perform <see cref="Redo"/> while a group operation is going on.</exception>
    /// <exception cref="InvalidOperationException">There is no action to redo.</exception>
    void Redo();

    /// <summary>
    /// Undoes the last executed <see cref="IUndo"/> command of the manager history.
    /// </summary>
    /// <exception cref="InvalidOperationException">Cannot perform <see cref="Undo"/> while a group operation is going on.</exception>
    /// <exception cref="InvalidOperationException">There is no action to undo.</exception>
    void Undo();
}
