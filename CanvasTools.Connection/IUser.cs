namespace CanvasTools.Connection;

/// <summary>
/// The details of a user.
/// </summary>
public interface IUser
    : IEquatable<IUser>, IComparable, IComparable<IUser>
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
    /// Lists the courses for the user.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>An <see cref="IAsyncEnumerable{ICourse}"/> containing the courses.</returns>
    IAsyncEnumerable<ICourse> ListCourses(CancellationToken cancellationToken = default);
}