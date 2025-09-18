using System.Diagnostics;

namespace CanvasTools.Connection.Http;

/// <summary>
/// A course in Canvas.
/// </summary>
[DebuggerDisplay($"{{{nameof(Name)}}} [{{{nameof(Id)}}}]")]
public class Course
    : ICourse, ILockable
{
    private ICanvas? _canvas;
    private string _code = string.Empty;
    private ulong _id;
    private string _name = string.Empty;
    private ITerm _term = new Term();

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
    /// The course code.
    /// </summary>
    public string Code
    {
        get => _code;
        set
        {
            if (IsLocked) throw new EntityLockedException();
            _code = value;
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
    /// The term that this course is offered.
    /// </summary>
    public ITerm Term
    {
        get => _term;
        set
        {
            if (IsLocked) throw new EntityLockedException();
            _term = value;
        }
    }

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
        return CompareTo(obj as ICourse);
    }

    /// <summary>
    /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
    /// </summary>
    /// <param name="other">An object to compare with this instance.</param>
    /// <returns>A value that indicates the relative order of the objects being compared. The return value has these meanings:
    /// <list type="table"><listheader><term> Value</term><description> Meaning</description></listheader><item><term> Less than zero</term><description> This instance precedes <paramref name="other" /> in the sort order.</description></item><item><term> Zero</term><description> This instance occurs in the same position in the sort order as <paramref name="other" />.</description></item><item><term> Greater than zero</term><description> This instance follows <paramref name="other" /> in the sort order.</description></item></list></returns>
    public int CompareTo(ICourse? other)
    {
        return string.CompareOrdinal(_code, other?.Code);
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
    public bool Equals(ICourse? other)
    {
        return other?.Id == _id;
    }

    /// <summary>
    /// Locks this entity.
    /// </summary>
    public void Lock()
    {
        IsLocked = true;
    }
}