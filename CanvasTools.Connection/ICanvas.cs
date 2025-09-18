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
    /// Lists the courses for the current user.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>An <see cref="IAsyncEnumerable{ICourse}"/> containing the courses.</returns>
    IAsyncEnumerable<ICourse> ListCoursesForCurrentUser(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the high level details of a course.
    /// </summary>
    /// <param name="courseId">The identifier of the course.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A <see cref="ICourse"/> instance, if found; <c>null</c> otherwise.</returns>
    Task<ICourse?> RetrieveCourse(ulong courseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the current user.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A <see cref="IUser"/> containing the current user's details.</returns>
    Task<IUser?> RetrieveCurrentUser(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a term for an account.
    /// </summary>
    /// <param name="accountId">The identifier of the account.</param>
    /// <param name="termId">The identifier of the term.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A <see cref="ICourse"/> instance, if found; <c>null</c> otherwise.</returns>
    Task<ITerm?> RetrieveTerm(ulong accountId, ulong termId, CancellationToken cancellationToken = default);
}