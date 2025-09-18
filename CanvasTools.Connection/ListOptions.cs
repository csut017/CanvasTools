namespace CanvasTools.Connection;

/// <summary>
/// The options for a list operation.
/// </summary>
/// <param name="OnProgressUpdate">A function to call whenever a new page of data is retrieved.</param>
/// <param name="MaxPages">The maximum number of pages to retrieve.</param>
public record ListOptions(Func<ProgressUpdate, bool>? OnProgressUpdate = null, int MaxPages = int.MaxValue);