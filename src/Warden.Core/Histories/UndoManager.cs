using System.ComponentModel;
using Warden.Core.Histories.Internal;

namespace Warden.Core.Histories;

/// <summary>
/// Provides an implementation of the command pattern to execute operations and return to a previous state by undoing them.
/// </summary>
public sealed class UndoManager : IUndoManager
{
    private sealed class Transaction : IUndoTransaction
    {
        private readonly UndoManager _manager;
        private readonly object? _description;
        private readonly List<IUndo> _commands;

        private bool _isCommitted;
        private bool _isDisposed;

        public Transaction(UndoManager manager, object? description)
        {
            _manager = manager;
            _description = description;
            _commands = [];

            _isCommitted = false;
            _isDisposed = false;

            _manager._transactions.Push(this);
        }

        public void Add(IUndo command) => _commands.Add(command);

        #region IUnDoTransaction

        public void Commit()
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);

            if (_isCommitted)
            {
                throw new InvalidOperationException(
                    "Current transaction has already been committed"
                );
            }

            if (_manager._transactions.Peek() != this)
            {
                throw new InvalidOperationException(
                    "Current transaction is not the highest one on the stack"
                );
            }

            _manager._transactions.Pop();
            if (_commands.Count > 0)
            {
                IUndo group = new GroupUndo(_description, [.. _commands]);
                if (_manager._transactions.Count > 0)
                {
                    _manager._transactions.Peek().Add(group);
                }
                else
                {
                    _manager.Push(group);
                }
            }

            _isCommitted = true;
        }

        #endregion

        #region IDisposable

        void IDisposable.Dispose()
        {
            if (!_isDisposed)
            {
                if (!_isCommitted)
                {
                    if (_manager._transactions.Peek() != this)
                    {
                        throw new InvalidOperationException(
                            "Current transaction is not the highest one on the stack"
                        );
                    }

                    _manager._transactions.Pop();

                    for (int i = _commands.Count - 1; i >= 0; --i)
                    {
                        _commands[i].Undo();
                    }
                }

                _isDisposed = true;
                // ReSharper disable once GCSuppressFinalizeForTypeWithoutDestructor
                GC.SuppressFinalize(this);
            }
        }

        #endregion
    }

    private readonly IUndoStack _stack;
    private readonly Stack<Transaction> _transactions;

    private int _cyclicDepth;
    private int _lastVersion;

    /// <summary>
    /// Initializes an instance of <see cref="UndoManager"/>.
    /// </summary>
    /// <param name="maxCapacity">The maximum number of operation this <see cref="UndoManager"/> can record before erasing the oldest ones.</param>
    public UndoManager(int maxCapacity)
    {
        if (maxCapacity <= 0)
        {
            throw new ArgumentException(
                "maxCapacity must be superior to zero",
                nameof(maxCapacity)
            );
        }

        _stack = maxCapacity == int.MaxValue ? new UndoStack() : new UndoBuffer(maxCapacity);
        _transactions = new Stack<Transaction>();

        _cyclicDepth = 0;
        Version = 0;
        _lastVersion = 0;
    }

    /// <summary>
    /// Initializes an instance of <see cref="UndoManager"/>.
    /// </summary>
    public UndoManager()
        : this(int.MaxValue) { }

    private void Push(IUndo command) => Version = _stack.Push(command, ++_lastVersion, Version);

    #region IUnDoManager

    /// <summary>
    /// Gets an <see cref="int"/> representing the state of the <see cref="IUndoManager"/>.
    /// </summary>
    public int Version
    {
        get;
        private set
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Version)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanUndo)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanRedo)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UndoDescriptions)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RedoDescriptions)));
        }
    }

    /// <summary>
    /// Returns a boolean to express if the method <see cref="Undo"/> can be executed.
    /// </summary>
    /// <returns>true if <see cref="Undo"/> can be executed, else false.</returns>
    public bool CanUndo => _stack.CanUndo;

    /// <summary>
    /// Returns a boolean to express if the method <see cref="Redo"/> can be executed.
    /// </summary>
    /// <returns>true if <see cref="Redo"/> can be executed, else false.</returns>
    public bool CanRedo => _stack.CanRedo;

    /// <summary>
    /// Gets the descriptions in order of all the <see cref="IUndo"/> which can be undone.
    /// </summary>
    public IEnumerable<object?> UndoDescriptions => _stack.UndoDescriptions;

    /// <summary>
    /// Gets the descriptions in order of all the <see cref="IUndo"/> which can be redone.
    /// </summary>
    public IEnumerable<object?> RedoDescriptions => _stack.RedoDescription;

    /// <summary>
    /// Starts a group of operation and return an <see cref="IUndoTransaction"/> to stop the group.
    /// If <see cref="IUndoTransaction.Commit"/> is not called, all operations done during the transaction will be undone on <see cref="IDisposable.Dispose"/>.
    /// </summary>
    /// <param name="description">The description of the group of operations.</param>
    /// <returns>An <see cref="IUndoTransaction"/> to commit or rollback the transaction of operations.</returns>
    public IUndoTransaction BeginTransaction(object? description = null) =>
        new Transaction(this, description);

    /// <summary>
    /// Clears the history of <see cref="IUndo"/> operations.
    /// </summary>
    /// <exception cref="InvalidOperationException">Cannot perform Clear while a transaction is going on.</exception>
    public void Clear()
    {
        if (_transactions.Count > 0)
        {
            throw new InvalidOperationException(
                "Cannot perform Clear while a transaction is going on."
            );
        }

        _stack.Clear();

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanUndo)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanRedo)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UndoDescriptions)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RedoDescriptions)));
    }

    /// <summary>
    /// Executes the <see cref="IUndo"/> command and stores it in the manager hostory.
    /// </summary>
    /// <param name="command">The <see cref="IUndo"/> to execute.</param>
    /// <exception cref="ArgumentNullException"><paramref name="command"/> is null.</exception>
    public void Do(IUndo command)
    {
        ArgumentNullException.ThrowIfNull(command);

        try
        {
            ++_cyclicDepth;
            command.Do();
        }
        finally
        {
            --_cyclicDepth;
        }

        if (_cyclicDepth is 0)
        {
            if (_transactions.Count > 0)
            {
                _transactions.Peek().Add(command);
            }
            else
            {
                Push(command);
            }
        }
    }

    /// <summary>
    /// Undoes the last executed <see cref="IUndo"/> command of the manager history.
    /// </summary>
    /// <exception cref="InvalidOperationException">Cannot perform <see cref="Undo"/> while a group operation is going on.</exception>
    /// <exception cref="InvalidOperationException">There is no action to undo.</exception>
    public void Undo()
    {
        if (_transactions.Count > 0)
        {
            throw new InvalidOperationException(
                "Cannot perform Undo while a transaction is going on."
            );
        }

        if (!CanUndo)
        {
            throw new InvalidOperationException("No operation to undo.");
        }

        try
        {
            ++_cyclicDepth;
            Version = _stack.Undo();
        }
        finally
        {
            --_cyclicDepth;
        }
    }

    /// <summary>
    /// Redoes the last undone <see cref="IUndo"/> command of the manager history.
    /// </summary>
    /// <exception cref="InvalidOperationException">Cannot perform <see cref="Redo"/> while a group operation is going on.</exception>
    /// <exception cref="InvalidOperationException">There is no action to redo.</exception>
    public void Redo()
    {
        if (_transactions.Count > 0)
        {
            throw new InvalidOperationException(
                "Cannot perform Redo while a transaction is going on."
            );
        }

        if (!CanRedo)
        {
            throw new InvalidOperationException("No operation to redo.");
        }

        try
        {
            ++_cyclicDepth;
            Version = _stack.Redo();
        }
        finally
        {
            --_cyclicDepth;
        }
    }

    #endregion

    #region INotifyPropertyChanged

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion
}
