using System.Collections.ObjectModel;
using Serilog.Events;

namespace CanvasTools.Windows.ViewModels;

/// <summary>
/// Exposes the event log to any views that want to display.
/// </summary>
public class InternalLoggingContext
{
    /// <summary>
    /// The events that have been logged.
    /// </summary>
    public ObservableCollection<LogEvent> Events { get; } = [];
}