namespace Warden.Core.Histories;

/// <summary>
/// Provides data for the operation about to be performed on an undo collection.
/// </summary>
/// <param name="Collection">The collection on which the action is performed.</param>
/// <param name="Action">The action performed.</param>
/// <param name="Parameters">The parameters of the action performed.</param>
public readonly record struct UndoCollectionOperation(
    object Collection,
    UndoCollectionAction Action,
    params object?[] Parameters
);
