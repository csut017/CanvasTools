using System.Diagnostics;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
using Serilog;

namespace CanvasTools.Connection.Http;

/// <summary>
/// A user in Canvas.
/// </summary>
[DebuggerDisplay($"{{{nameof(Name)}}} [{{{nameof(Id)}}}]")]
public class User
    : IUser, ILockable
{
    private ICanvas? _canvas;
    private ulong _id;
    private string _name = string.Empty;

    /// <summary>
    /// The associated Canvas entity.
    /// </summary>
    public ICanvas? Canvas
    {
        get => _canvas;
        set
        {
            if (IsLocked) throw new EntityLockedException();
            _canvas = value;
        }
    }

    /// <summary>
    /// The identifier of the course.
    /// </summary>
    public ulong Id
    {
        get => _id;
        set
        {
            if (IsLocked) throw new EntityLockedException();
            _id = value;
        }
    }

    /// <summary>
    /// A flag indicating whether this entity has been locked or not.
    /// </summary>
    public bool IsLocked { get; private set; }

    /// <summary>
    /// The name of the course.
    /// </summary>
    public string Name
    {
        get => _name;
        set
        {
            if (IsLocked) throw new EntityLockedException();
            _name = value;
        }
    }

    /// <summary>
    /// An associated logger.
    /// </summary>
    internal ILogger? Logger { get; init; }

    /// <summary>
    /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
    /// </summary>
    /// <param name="obj">An object to compare with this instance.</param>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="obj" /> is not the same type as this instance.</exception>
    /// <returns>A value that indicates the relative order of the objects being compared. The return value has these meanings:
    /// <list type="table"><listheader><term> Value</term><description> Meaning</description></listheader><item><term> Less than zero</term><description> This instance precedes <paramref name="obj" /> in the sort order.</description></item><item><term> Zero</term><description> This instance occurs in the same position in the sort order as <paramref name="obj" />.</description></item><item><term> Greater than zero</term><description> This instance follows <paramref name="obj" /> in the sort order.</description></item></list></returns>
    public int CompareTo(object? obj)
    {
        return CompareTo(obj as IUser);
    }

    /// <summary>Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.</summary>
    /// <param name="other">An object to compare with this instance.</param>
    /// <returns>A value that indicates the relative order of the objects being compared. The return value has these meanings:
    /// <list type="table"><listheader><term> Value</term><description> Meaning</description></listheader><item><term> Less than zero</term><description> This instance precedes <paramref name="other" /> in the sort order.</description></item><item><term> Zero</term><description> This instance occurs in the same position in the sort order as <paramref name="other" />.</description></item><item><term> Greater than zero</term><description> This instance follows <paramref name="other" /> in the sort order.</description></item></list></returns>
    public int CompareTo(IUser? other)
    {
        return string.Compare(_name, other?.Name, StringComparison.Ordinal);
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
    public bool Equals(IUser? other)
    {
        return other?.Id == _id;
    }

    /// <summary>
    /// Lists the courses for the user.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>An <see cref="IAsyncEnumerable{ICourse}"/> containing the courses.</returns>
    public async IAsyncEnumerable<ICourse> ListCourses([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        Guard.IsNotNull(Canvas);
        Logger?.Debug("Retrieving courses for {user}", _id);
        await foreach (var item in Canvas.Connection.List<Course>($"api/v1/users/{Id}/courses",
            new Parameters(),
            cancellationToken: cancellationToken))
        {
            item.Canvas = Canvas;
            item.Lock();
            yield return item;
        }
    }

    /// <summary>
    /// Locks this entity.
    /// </summary>
    public void Lock()
    {
        IsLocked = true;
    }
}