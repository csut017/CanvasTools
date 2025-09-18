namespace CanvasTools.Connection;

/// <summary>
/// The high level details of a course.
/// </summary>
public interface ICourse
    : IEquatable<ICourse>, IComparable, IComparable<ICourse>
{
    /// <summary>
    /// The course code.
    /// </summary>
    string Code { get; }

    /// <summary>
    /// The identifier of the course.
    /// </summary>
    ulong Id { get; }

    /// <summary>
    /// The name of the course.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The term that this course is offered.
    /// </summary>
    ITerm Term { get; }
}