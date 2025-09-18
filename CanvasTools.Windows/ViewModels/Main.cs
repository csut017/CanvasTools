using System.Windows;
using CanvasTools.Connection;
using CanvasTools.Windows.Interfaces;
using CanvasTools.Windows.ViewModels.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace CanvasTools.Windows.ViewModels;

/// <summary>
/// The view model for the application's main window.
/// </summary>
public partial class Main(IShell shell, ILogger logger, IWindowManager windowManager, IServiceProvider serviceProvider, ICanvas canvas)
    : ObservableObject
{
    private IWindow? _logsView;
    public IShell Shell => shell;

    /// <summary>
    /// Initialises the main window.
    /// </summary>
    public void Initialise()
    {
        shell.UpdateStatus("Initialising main...", true);
        IUser? user = null;
        Task.Run(async () =>
            {
                shell.UpdateStatus("Retrieving user details from Canvas...", true);
                user = await canvas.RetrieveCurrentUser();
            })
            .ContinueWith(t =>
            {
                shell.CurrentUser = user;
                shell.UpdateStatus("Initialised main");
                shell.InitialiseCurrentView();
            }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    [RelayCommand]
    private void Exit()
    {
        logger.Information("Exiting application");
        Application.Current.Shutdown();
    }

    [RelayCommand]
    private void ShowLog()
    {
        if (_logsView == null)
        {
            var viewModel = serviceProvider.GetRequiredService<LogsDisplay>();
            _logsView = windowManager.Get(viewModel, this);
        }

        _logsView.Show();
    }
}