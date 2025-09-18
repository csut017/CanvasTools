using System.Windows;
using CanvasTools.Windows.Interfaces;
using CanvasTools.Windows.ViewModels.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace CanvasTools.Windows.ViewModels;

public partial class Main(IShell shell, ILogger logger, IWindowManager windowManager, IServiceProvider serviceProvider)
    : ObservableObject
{
    private IWindow? _logsView;
    public IShell Shell => shell;

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