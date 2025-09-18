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
    /// Retrieves the high level details of a course.
    /// </summary>
    /// <param name="courseId">The identifier of the course.</param>
    /// <returns>A <see cref="ICourse"/> instance, if found; <c>null</c> otherwise.</returns>
    public async Task<ICourse?> RetrieveCourse(ulong courseId)
    {
        Logger?.Debug("Retrieving course with courseId {courseId}", courseId);
        var item = await Connection.Retrieve<Course>(
            $"/api/v1/courses/{courseId}",
            new Parameters());
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
    /// <returns>A <see cref="ICourse"/> instance, if found; <c>null</c> otherwise.</returns>
    public async Task<ITerm?> RetrieveTerm(ulong accountId, ulong termId)
    {
        Logger?.Debug("Retrieving term with id {termId} in {accountId}", termId, accountId);
        var item = await Connection.Retrieve<Term>(
            $"/api/v1/accounts/{accountId}/terms/{termId}",
            new Parameters());
        if (item == null) return null;
        item.Canvas = this;
        item.Lock();
        return item;
    }
}