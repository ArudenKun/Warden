namespace Warden.Core.Histories.Internal;

internal interface IUndoStack
{
    bool CanUndo { get; }

    bool CanRedo { get; }

    IEnumerable<object?> UndoDescriptions { get; }

    IEnumerable<object?> RedoDescription { get; }

    int Push(IUndo command, int doVersion, int undoVersion);

    int Undo();

    int Redo();

    void Clear();
}
