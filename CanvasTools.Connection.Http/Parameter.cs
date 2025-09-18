using System.Diagnostics;

namespace CanvasTools.Connection.Http;

/// <summary>
/// A parameter for an API call.
/// </summary>
[DebuggerDisplay($"{{{nameof(Name)}}}={{{nameof(Value)}}}")]
public class Parameter
{
    /// <summary>
    /// The name of the parameter.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The value of the parameter.
    /// </summary>
    public string Value { get; set; } = string.Empty;
}