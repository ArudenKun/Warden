using Warden.Core.FastUndoRedo;

namespace Warden.Core;

public class ViewModelUndoRedoAdapter
{
    private readonly RegistrationTracker _tracker;

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelUndoRedoAdapter"/> class.
    /// </summary>
    /// <param name="service">Undo/redo service instance.</param>
    public ViewModelUndoRedoAdapter(UndoRedoService service)
    {
        _tracker = new RegistrationTracker(service);
    }

    /// <summary>
    /// Register a view-model or object for change tracking.
    /// </summary>
    /// <param name="vm">Object to register.</param>
    public void Register(object vm) => _tracker.Register(vm);

    /// <summary>
    /// Unregister a previously registered view-model or object.
    /// </summary>
    /// <param name="vm">Object to unregister.</param>
    public void Unregister(object vm) => _tracker.Unregister(vm);
}
