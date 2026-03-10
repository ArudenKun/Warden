namespace Warden.Core.Histories.Internals;

internal interface IUndoStack
{
    bool CanUndo { get; }

    bool CanRedo { get; }

    IEnumerable<object?> UndoDescriptions { get; }

    IEnumerable<object?> RedoDescription { get; }

    int Push(IUndo command, int executeVersion, int undoVersion);

    int Undo();

    int Redo();

    void Clear();
}
