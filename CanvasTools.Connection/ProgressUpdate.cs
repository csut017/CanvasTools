namespace CanvasTools.Connection;

/// <summary>
/// The details of a progress update.
/// </summary>
/// <param name="ItemCount">The number of items retrieved.</param>
/// <param name="PageCount">The number of pages retrieved.</param>
public record ProgressUpdate(int ItemCount, int PageCount);