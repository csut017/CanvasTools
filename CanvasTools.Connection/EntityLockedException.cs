namespace CanvasTools.Connection;

/// <summary>
/// An exception for when a locked entity is being modified.
/// </summary>
public class EntityLockedException()
    : Exception("This entity has been locked");