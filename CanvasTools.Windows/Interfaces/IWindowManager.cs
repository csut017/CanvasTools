namespace CanvasTools.Windows.Interfaces;

public interface IWindowManager
{
    IWindow Get<TViewModel>(TViewModel viewModel, object? parent = null);

    bool? Show<TViewModel>(TViewModel viewModel, object? parent);
}