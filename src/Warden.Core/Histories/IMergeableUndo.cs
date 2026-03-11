using System.Diagnostics.CodeAnalysis;

namespace Warden.Core.Histories;

/// <summary>
/// Provides a method to try to merge two <see cref="IUndo"/> instances into a single one.
/// </summary>
public interface IMergeableUndo : IUndo
{
    /// <summary>
    /// Merges the current instance with the specified <see cref="IUndo"/>.
    /// </summary>
    /// <param name="other">The other <see cref="IUndo"/> instance the current one should try to merge with.</param>
    /// <param name="mergedCommand">The resulting merged <see cref="IUndo"/> instance if the operation was successful.</param>
    /// <returns>true if the merge was successful; otherwise false.</returns>
    bool TryMerge(IUndo other, [NotNullWhen(true)] out IUndo? mergedCommand);
}
