namespace CanvasTools.Connection;

/// <summary>
/// The parameters for an API call.
/// </summary>
public interface IParameters
{
    /// <summary>
    /// Convert to a <see langword="string"/> containing the query parameters.
    /// </summary>
    /// <returns>A <see langword="string"/> containing the query parameters.</returns>
    string ToQueryString();
}