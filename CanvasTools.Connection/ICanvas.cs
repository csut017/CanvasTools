namespace CanvasTools.Connection;

/// <summary>
/// The root level connection to a Canvas instance.
/// </summary>
public interface ICanvas
{
    /// <summary>
    /// The underlying connection.
    /// </summary>
    IConnection Connection { get; }

    /// <summary>
    /// Retrieves the high level details of a course.
    /// </summary>
    /// <param name="courseId">The identifier of the course.</param>
    /// <returns>A <see cref="ICourse"/> instance, if found; <c>null</c> otherwise.</returns>
    Task<ICourse?> RetrieveCourse(ulong courseId);

    /// <summary>
    /// Retrieves a term for an account.
    /// </summary>
    /// <param name="accountId">The identifier of the account.</param>
    /// <param name="termId">The identifier of the term.</param>
    /// <returns>A <see cref="ICourse"/> instance, if found; <c>null</c> otherwise.</returns>
    Task<ITerm?> RetrieveTerm(ulong accountId, ulong termId);
}