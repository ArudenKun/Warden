namespace Warden.Core.Histories.Internals;

internal readonly record struct Operation(IUndo Command, int ExecuteVersion, int UndoVersion);
