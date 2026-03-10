using System.Diagnostics.CodeAnalysis;

namespace Warden.Core.Histories;

/// <summary>
/// Provides a simple wrapper for a field to automatically generate <see cref="IUndo"/> operations.
/// </summary>
/// <typeparam name="T">The type of the filed.</typeparam>
public class UndoField<T>
{
    private readonly IHistory _manager;
    private readonly Func<UndoFieldChange<T>, object?>? _descriptionFactory;

    [AllowNull]
    private T _value;

    /// <summary>
    /// Gets or sets the value of the field, generating a <see cref="IUndo"/> operation on set.
    /// </summary>
    [AllowNull]
    public T Value
    {
        get => _value;
        set =>
            _manager.Execute(
                Set,
                value,
                _value,
                _descriptionFactory?.Invoke(new UndoFieldChange<T>(_value, value))
            );
    }

    /// <summary>
    /// Creates a new instance of <see cref="UndoField{T}"/>.
    /// </summary>
    /// <param name="manager">The <see cref="IHistory"/> used to register the <see cref="IUndo"/> operations.</param>
    /// <param name="value">The starting value of the <see cref="UndoField{T}"/></param>
    /// <param name="descriptionFactory">Factory used to create the description of the generated <see cref="IUndo"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="manager"/> is null.</exception>
    public UndoField(
        IHistory manager,
        [AllowNull] T value,
        Func<UndoFieldChange<T>, object?>? descriptionFactory = null
    )
    {
        ArgumentNullException.ThrowIfNull(manager);

        _manager = manager;
        _descriptionFactory = descriptionFactory;

        _value = value;
    }

    /// <summary>
    /// Creates a new instance of <see cref="UndoField{T}"/>.
    /// </summary>
    /// <param name="manager">The <see cref="IHistory"/> used to register the <see cref="IUndo"/> operations.</param>
    /// <param name="descriptionFactory">Factory used to create the description of the generated <see cref="IUndo"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="manager"/> is null.</exception>
    public UndoField(IHistory manager, Func<UndoFieldChange<T>, object?>? descriptionFactory = null)
        : this(manager, default, descriptionFactory) { }

    private void Set([AllowNull] T value)
    {
        PreSet(value);

        T oldValue = _value;
        _value = value;

        PostSet(oldValue);
    }

    /// <summary>
    /// Performs a pre set treatment, included in the <see cref="IUndo"/> operation.
    /// </summary>
    /// <param name="newValue">The new value.</param>
    protected virtual void PreSet([AllowNull] T newValue) { }

    /// <summary>
    /// performs a post set treatment, included in the <see cref="IUndo"/> operation.
    /// </summary>
    /// <param name="oldValue">The old value.</param>
    protected virtual void PostSet([AllowNull] T oldValue) { }

    /// <summary>
    ///Defines an implicit conversion of a <see cref="UndoField{T}"/> to a <typeparamref name="T"/>.
    /// </summary>
    /// <param name="field">The field to implicitely convert.</param>
    [return: MaybeNull]
    [SuppressMessage(
        "Usage",
        "CA2225:Operator overloads have named alternates",
        Justification = "Value property"
    )]
    public static implicit operator T(UndoField<T> field) => field is null ? default : field.Value;
}
