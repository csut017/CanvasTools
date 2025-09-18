using CanvasTools.Windows.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CanvasTools.Windows.ViewModels;

public partial class Shell
    : ObservableObject, IShell
{
    /// <summary>
    /// The current view.
    /// </summary>
    [ObservableProperty]
    private object? _currentView;

    /// <summary>
    /// A flag indicating whether the last action has an associated file that can opened.
    /// </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenAssociatedFileCommand))]
    private bool _hasAssociatedFile;

    /// <summary>
    /// A flag indicating whether the application is loading.
    /// </summary>
    [ObservableProperty]
    private bool _isLoading;

    /// <summary>
    /// Opens the associated file.
    /// </summary>
    [RelayCommand(CanExecute = nameof(HasAssociatedFile))]
    private Task OpenAssociatedFile()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Initialises a new <see cref="Shell"/> instance.
    /// </summary>
    public Shell()
    {
        _progress = new Progress<StatusUpdate>(OnUpdateStatus);
    }

    /// <summary>
    /// The text about the current status.
    /// </summary>
    [ObservableProperty]
    private string _statusText = string.Empty;

    /// <summary>
    /// The current view stack.
    /// </summary>
    private readonly Stack<object> _viewStack = [];

    /// <summary>
    /// Displays a new view.
    /// </summary>
    /// <param name="view">The view to display.</param>
    public void DisplayView(object view)
    {
        if (CurrentView != null) _viewStack.Push(CurrentView);
        CurrentView = view;
    }

    /// <summary>
    /// Restores the previous view.
    /// </summary>
    public void RestoreView()
    {
        if (_viewStack.Count == 0) return;
        var previous = _viewStack.Pop();
        CurrentView = previous;
    }

    /// <summary>
    /// Updates the current status.
    /// </summary>
    /// <param name="statusText">The text for the status.</param>
    /// <param name="isLoading">A flag indicating whether the application is in a loading mode or not.</param>
    /// <param name="associatedFile">An associated file that can be opened by the user.</param>
    public void UpdateStatus(string statusText, bool isLoading = false, string? associatedFile = null)
    {
        _progress.Report(new(statusText, isLoading, associatedFile));
    }

    private readonly IProgress<StatusUpdate> _progress;

    private string? _associatedFile;

    private void OnUpdateStatus(StatusUpdate status)
    {
        StatusText = status.Text;
        IsLoading = status.IsLoading;
        _associatedFile = status.AssociatedFile;
        HasAssociatedFile = !string.IsNullOrEmpty(_associatedFile);
    }

    private record StatusUpdate(string Text, bool IsLoading, string? AssociatedFile);
}