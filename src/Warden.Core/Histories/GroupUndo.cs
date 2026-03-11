using System.Diagnostics.CodeAnalysis;
using Warden.Core.Histories.Internal.Extensions;

namespace Warden.Core.Histories;

/// <summary>
/// Provides an implementation of the <see cref="IUndo"/> interface for a group of operations.
/// </summary>
public sealed class GroupUndo : IMergeableUndo
{
    /// <summary>
    /// Represents a method that will be called when merging a <see cref="GroupUndo"/> and a <see cref="IMergeableUndo"/> instances to get the resulting description.
    /// </summary>
    /// <param name="oldDescription">The description of the previous <see cref="GroupUndo"/> merged.</param>
    /// <param name="newDescription">The description of the new <see cref="IMergeableUndo"/> merged.</param>
    /// <param name="mergedDescription">The description of the new resulting <see cref="IMergeableUndo"/> merged.</param>
    /// <returns>The final description to use.</returns>
    public delegate object MergeDescriptionHandler(
        object? oldDescription,
        object? newDescription,
        object? mergedDescription
    );

    private readonly object? _description;
    private readonly IUndo[] _commands;

    /// <summary>
    /// The <see cref="MergeDescriptionHandler"/> used to merge description between a <see cref="GroupUndo"/> and a <see cref="IMergeableUndo"/> instances.
    /// </summary>
    public static MergeDescriptionHandler? MergeDescriptionAction { get; set; }

    /// <summary>
    /// Initialise an instance of <see cref="GroupUndo"/>.
    /// </summary>
    /// <param name="description">The description of this <see cref="IUndo"/></param>
    /// <param name="commands">The sequence of <see cref="IUndo"/> contained by the instance.</param>
    /// <exception cref="ArgumentException"><paramref name="commands"/> contains null elements.</exception>
    public GroupUndo(object? description, params ReadOnlySpan<IUndo> commands)
    {
        if (commands.Length is 0)
        {
            throw new ArgumentException("IUnDo sequence contains no elements.", nameof(commands));
        }

        if (commands.Any(i => i is null))
        {
            throw new ArgumentException("IUnDo sequence contains null elements.", nameof(commands));
        }

        _description = description;
        _commands = [.. commands];
    }

    /// <summary>
    /// Initialise an instance of <see cref="GroupUndo"/>.
    /// </summary>
    /// <param name="commands">The sequence of <see cref="IUndo"/> contained by the instance.</param>
    /// <exception cref="ArgumentNullException"><paramref name="commands"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="commands"/> contains null elements.</exception>
    public GroupUndo(params ReadOnlySpan<IUndo> commands)
        : this(null, commands) { }

    /// <summary>
    /// Gets the single <typeparamref name="T"/> of this instance.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="IUndo"/> to get.</typeparam>
    /// <param name="command">
    /// When this method returns, the single <typeparamref name="T"/> of this instance, if it was its only command; otherwise, the default value for the type <typeparamref name="T"/>.
    /// This parameter is passed uninitialized.
    /// </param>
    /// <returns>true if the current instance contains exactly one <typeparamref name="T"/>; otherwise false.</returns>
    public bool TryGetSingle<T>([NotNullWhen(true)] out T command)
        where T : class, IUndo
    {
        command = _commands.Length is 1 && _commands[0] is T t ? t : default!;

        return command != null;
    }

    #region IMergeableUnDo

    /// <inheritdoc />
    bool IMergeableUndo.TryMerge(IUndo other, [NotNullWhen(true)] out IUndo? mergedCommand)
    {
        mergedCommand =
            TryGetSingle(out IMergeableUndo mergeable)
            && mergeable.TryMerge(other, out mergedCommand)
                ? new GroupUndo(
                    MergeDescriptionAction?.Invoke(
                        _description,
                        mergeable.Description,
                        mergedCommand.Description
                    ) ?? _description,
                    mergedCommand
                )
                : null;

        return mergedCommand != null;
    }

    #endregion

    #region IUnDo

    /// <inheritdoc />
    object? IUndo.Description => _description;

    /// <inheritdoc />
    void IUndo.Do()
    {
        foreach (IUndo command in _commands)
        {
            command.Do();
        }
    }

    /// <inheritdoc />
    void IUndo.Undo()
    {
        for (int i = _commands.Length - 1; i >= 0; --i)
        {
            _commands[i].Undo();
        }
    }

    #endregion
}
