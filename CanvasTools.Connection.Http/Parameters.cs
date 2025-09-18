using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace CanvasTools.Connection.Http;

/// <summary>
/// The parameters to pass to an API call.
/// </summary>
[DebuggerDisplay($"{{{nameof(DebuggerInfo)}}}")]
public class Parameters
    : List<Parameter>, IParameters
{
    [ExcludeFromCodeCoverage]
    private string DebuggerInfo => ToQueryString();

    /// <summary>
    /// Convert to a <see langword="string"/> containing the query parameters.
    /// </summary>
    /// <returns>A <see langword="string"/> containing the query parameters.</returns>
    public string ToQueryString()
    {
        if (Count == 0) return string.Empty;
        var output = "?" + string.Join("&", this.Select(p => $"{p.Name}={WebUtility.UrlEncode(p.Value)}"));
        return output;
    }
}