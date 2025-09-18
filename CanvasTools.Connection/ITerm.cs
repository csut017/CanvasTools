namespace CanvasTools.Connection;

/// <summary>
/// The details of a term.
/// </summary>
public interface ITerm
    : IEquatable<ITerm>, IComparable, IComparable<ITerm>
{
    /// <summary>
    /// The identifier of the course.
    /// </summary>
    ulong Id { get; }

    /// <summary>
    /// The name of the course.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// When the term starts.
    /// </summary>
    DateTime? WhenEnds { get; }

    /// <summary>
    /// When the term starts.
    /// </summary>
    DateTime? WhenStarts { get; }
}