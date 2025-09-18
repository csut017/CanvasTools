namespace CanvasTools.Connection.Http;

/// <summary>
/// Marks an entity as being lockable (to prevent updates).
/// </summary>
public interface ILockable
{
    /// <summary>
    /// A flag indicating whether this entity has been locked or not.
    /// </summary>
    bool IsLocked { get; }

    /// <summary>
    /// Locks this entity.
    /// </summary>
    void Lock();
}