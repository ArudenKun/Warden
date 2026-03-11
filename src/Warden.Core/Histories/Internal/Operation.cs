namespace Warden.Core.Histories.Internal;

internal readonly record struct Operation(IUndo Command, int DoVersion, int UndoVersion);
