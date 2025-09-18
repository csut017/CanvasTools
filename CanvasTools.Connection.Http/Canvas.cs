using System.Runtime.CompilerServices;
using Serilog;

namespace CanvasTools.Connection.Http;

/// <summary>
/// The root level connection to a Canvas instance.
/// </summary>
public sealed class Canvas
    : ICanvas
{
    /// <summary>
    /// The underlying connection.
    /// </summary>
    public required IConnection Connection { get; init; }

    /// <summary>
    /// An associated logger.
    /// </summary>
    internal ILogger? Logger { get; init; }

    /// <summary>
    /// Lists the courses for the current user.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>An <see cref="IAsyncEnumerable{ICourse}"/> containing the courses.</returns>
    public async IAsyncEnumerable<ICourse> ListCoursesForCurrentUser([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        Logger?.Debug("Retrieving courses for current user");
        await foreach (var item in Connection.List<Course>("api/v1/courses",
                           new Parameters(),
                           cancellationToken: cancellationToken))
        {
            item.Canvas = this;
            item.Lock();
            yield return item;
        }
    }

    /// <summary>
    /// Retrieves the high level details of a course.
    /// </summary>
    /// <param name="courseId">The identifier of the course.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A <see cref="ICourse"/> instance, if found; <c>null</c> otherwise.</returns>
    public async Task<ICourse?> RetrieveCourse(ulong courseId, CancellationToken cancellationToken = default)
    {
        Logger?.Debug("Retrieving course with courseId {courseId}", courseId);
        var item = await Connection.Retrieve<Course>(
            $"/api/v1/courses/{courseId}",
            new Parameters(),
            cancellationToken);
        if (item == null) return null;
        item.Canvas = this;
        item.Lock();
        return item;
    }

    /// <summary>
    /// Retrieves the current user.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A <see cref="IUser"/> containing the current user's details.</returns>
    public async Task<IUser?> RetrieveCurrentUser(CancellationToken cancellationToken = default)
    {
        Logger?.Debug("Retrieving current user");
        var item = await Connection.Retrieve<User>(
            $"/api/v1/users/self",
            new Parameters(),
            cancellationToken);
        if (item == null) return null;
        item.Canvas = this;
        item.Lock();
        return item;
    }

    /// <summary>
    /// Retrieves a term for an account.
    /// </summary>
    /// <param name="accountId">The identifier of the account.</param>
    /// <param name="termId">The identifier of the term.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A <see cref="ICourse"/> instance, if found; <c>null</c> otherwise.</returns>
    public async Task<ITerm?> RetrieveTerm(ulong accountId, ulong termId, CancellationToken cancellationToken = default)
    {
        Logger?.Debug("Retrieving term with id {termId} in {accountId}", termId, accountId);
        var item = await Connection.Retrieve<Term>(
            $"/api/v1/accounts/{accountId}/terms/{termId}",
            new Parameters(),
            cancellationToken);
        if (item == null) return null;
        item.Canvas = this;
        item.Lock();
        return item;
    }
}