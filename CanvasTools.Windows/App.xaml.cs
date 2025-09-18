using System.Windows;
using System.Windows.Controls;
using CanvasTools.Windows.Interfaces;
using CanvasTools.Windows.Internal;
using CanvasTools.Windows.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CanvasTools.Windows;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Initialise the application.
    /// </summary>
    private void OnStartup(object sender, StartupEventArgs e)
    {
        var settings = AppSettings.New<App>("appsettings.json");
        var serviceCollection = new ServiceCollection();
        var logger = serviceCollection.InitializeLogging("logs\\log-.txt");

        logger.Information("Registering core views");
        EventManager.RegisterClassHandler(typeof(TextBox), UIElement.GotFocusEvent, new RoutedEventHandler(TextBox_GotFocus));
        ViewsHelper.RegisterViews<App>(Resources, serviceCollection);

        logger.Information("Initializing services");
        serviceCollection.InitializeCanvas(settings.CanvasUrl, settings.CanvasToken);
        serviceCollection.AddSingleton<IShell, Shell>();
        serviceCollection.AddSingleton<IWindowManager, WindowManager>();
        var serviceProvider = serviceCollection.BuildServiceProvider();

        logger.Information("Initializing shell");
        var main = serviceProvider.GetRequiredService<Main>();
        var view = new Views.Main
        {
            DataContext = main
        };

        logger.Information("Displaying shell");
        view.Show();
        var initialView = serviceProvider.GetRequiredService<CourseList>();
        var shell = serviceProvider.GetRequiredService<IShell>();
        shell.UpdateStatus("Application started");
        shell.DisplayView(initialView);
        main.Initialise();
    }

    /// <summary>
    /// Select all the text in a textbox when the textbox gets focus.
    /// </summary>
    /// <param name="sender">The sender (should be a <see cref="TextBox"/>)</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance.</param>
    private void TextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        (sender as TextBox)?.SelectAll();
    }
}