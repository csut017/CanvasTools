using CommunityToolkit.Mvvm.Input;

namespace CanvasTools.Windows.Interfaces;

/// <summary>
/// Exposes core functionality to the view models and allows them to interact with the shell.
/// </summary>
public interface IShell
{
    /// <summary>
    /// The current view in the shell.
    /// </summary>
    object? CurrentView { get; }

    /// <summary>
    /// A flag indicating whether the last action has an associated file that can opened.
    /// </summary>
    bool HasAssociatedFile { get; }

    /// <summary>
    /// A flag indicating whether the application is loading.
    /// </summary>
    bool IsLoading { get; }

    /// <summary>
    /// Opens the associated file.
    /// </summary>
    IAsyncRelayCommand OpenAssociatedFileCommand { get; }

    /// <summary>
    /// The text about the current status.
    /// </summary>
    string StatusText { get; }

    /// <summary>
    /// Displays a new view.
    /// </summary>
    /// <param name="view">The view to display.</param>
    void DisplayView(object view);

    /// <summary>
    /// Restores the previous view.
    /// </summary>
    void RestoreView();

    /// <summary>
    /// Updates the current status.
    /// </summary>
    /// <param name="statusText">The text for the status.</param>
    /// <param name="isLoading">A flag indicating whether the application is in a loading mode or not.</param>
    /// <param name="associatedFile">An associated file that can be opened by the user.</param>
    void UpdateStatus(string statusText, bool isLoading = false, string? associatedFile = null);
}