using System.Reflection;
using System.Windows;
using CanvasTools.Windows.Interfaces;
using Serilog;

namespace CanvasTools.Windows.Internal;

internal class WindowManager(ILogger logger)
    : IWindowManager
{
    private readonly Dictionary<Assembly, Dictionary<Type, Type>> _views = [];

    public IWindow Get<TViewModel>(TViewModel viewModel, object? parent = null)
    {
        var win = new WindowWrapper<TViewModel>(
            () => InitializeView(
                viewModel,
                () => parent == null ? Application.Current.MainWindow : FindWindowForViewModel(parent)),
            viewModel);
        return win;
    }

    public bool? Show<TViewModel>(TViewModel viewModel, object? parent)
    {
        var view = InitializeView(
            viewModel,
            () => parent == null ? null : FindWindowForViewModel(parent));
        return view.ShowDialog();
    }

    private static Window? FindWindowForViewModel(object? parent)
    {
        foreach (var window in Application.Current.Windows)
        {
            if (window is not Window win) continue;
            if (win.DataContext == parent)
            {
                return win;
            }
        }

        return null;
    }

    private static Dictionary<Type, Type> LoadViewMappings()
    {
        var baseNamespaceLength = typeof(App).Namespace?.Length + 1 ?? 0;
        var allTypes = typeof(App).Assembly
            .GetTypes()
            .Where(t => t.FullName != null && t.FullName.Length > baseNamespaceLength)
            .ToDictionary(t => t.FullName![baseNamespaceLength..]);
        var viewMappings = new Dictionary<Type, Type>();
        foreach (var viewModel in allTypes.Where(t => t.Key.StartsWith("ViewModels.")))
        {
            var viewName = viewModel.Key.Replace("ViewModels.", "Views.");
            if (!allTypes.TryGetValue(viewName, out var viewType) || !viewType.IsAssignableTo(typeof(Window))) continue;
            viewMappings.Add(viewModel.Value, viewType);
        }

        return viewMappings;
    }

    private Window InitializeView<TViewModel>(TViewModel viewModel, Func<Window?> retrieveOwner)
    {
        var assembly = typeof(TViewModel).Assembly;
        if (!_views.TryGetValue(assembly, out var assemblyViews))
        {
            logger.Information("Loading view mappings from {assembly}", assembly.FullName);
            assemblyViews = LoadViewMappings();
            _views[assembly] = assemblyViews;
        }
        if (!assemblyViews.TryGetValue(typeof(TViewModel), out var viewType)) throw new InvalidOperationException($"No matching view for {typeof(TViewModel).Name}");

        if (Activator.CreateInstance(viewType) is not Window view) throw new InvalidOperationException($"Unable to initialize view {viewType.Name}");
        logger.Information("Initializing view {view}", typeof(TViewModel).FullName);
        view.DataContext = viewModel;
        view.Owner = retrieveOwner();
        return view;
    }

    private class WindowWrapper<TViewModel>(Func<Window> windowFunc, TViewModel? viewModel)
        : IWindow
    {
        private Window? _window;

        public TViewModel? ViewModel { get; } = viewModel;

        public void Close()
        {
            _window?.Close();
        }

        public void Show()
        {
            if (_window == null)
            {
                _window = windowFunc();
                _window.Closed += (o, e) => _window = null;
            }

            _window.Show();
        }
    }
}