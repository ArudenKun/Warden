using CommunityToolkit.Mvvm.Input;
using JetBrains.Annotations;
using SukiUI.Controls;
using SukiUI.Dialogs;

namespace Warden.Core;

public abstract class DialogViewModelBase : DialogViewModelBase<bool>;

[PublicAPI]
public abstract partial class DialogViewModelBase<TResult> : ViewModelBase
{
    private bool _isResultSet;

    protected DialogViewModelBase()
    {
        Dialog = new SukiDialog();
    }

    public TaskCompletionSource<bool> Completion { get; private set; } = new();

    protected ISukiDialog Dialog { get; private set; }

    protected ISukiDialogManager? Manager => Dialog.Manager;

    public TResult? DialogResult { get; private set; }

    /// <summary>
    /// Gets the title of the dialog.
    /// </summary>
    public virtual string DialogTitle => string.Empty;

    public override void OnLoaded()
    {
        Reset();
    }

    public override void OnUnloaded()
    {
        Reset();
    }

    [RelayCommand]
    protected void Close(TResult? result = default)
    {
        DialogResult = result;
        Completion.SetResult(result is not null);
        _isResultSet = true;
        Dialog.Dismiss();
    }

    public void SetDialog(ISukiDialog dialog)
    {
        Dialog = dialog;
    }

    protected void Reset()
    {
        if (!_isResultSet)
            return;

        Completion = new TaskCompletionSource<bool>();
        DialogResult = default;
        _isResultSet = false;
    }
}
