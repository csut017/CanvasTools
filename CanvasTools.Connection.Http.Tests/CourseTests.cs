namespace CanvasTools.Connection.Http.Tests;

[TestSubject(typeof(Course))]
public class CourseTests
{
    public static TheoryData<object> InvalidCourses => [(object)null, (object)"garbage", new object()];

    [Fact]
    public void CanSetCanvasWhenUnlocked()
    {
        // Arrange
        var value = A.Fake<ICanvas>();
        var entity = new Course { Canvas = null };

        // Act
        entity.Canvas = value;

        // Assert
        entity.Canvas.ShouldBe(value);
    }

    [Fact]
    public void CanSetCodeWhenUnlocked()
    {
        // Arrange
        const string value = "new code";
        var entity = new Course { Code = "old code" };

        // Act
        entity.Code = value;

        // Assert
        entity.Code.ShouldBe(value);
    }

    [Fact]
    public void CanSetIdWhenUnlocked()
    {
        // Arrange
        ulong value = 123;
        var entity = new Course { Id = 456 };

        // Act
        entity.Id = value;

        // Assert
        entity.Id.ShouldBe(value);
    }

    [Fact]
    public void CanSetNameWhenUnlocked()
    {
        // Arrange
        const string value = "new name";
        var entity = new Course { Name = "old name" };

        // Act
        entity.Name = value;

        // Assert
        entity.Name.ShouldBe(value);
    }

    [Fact]
    public void CanSetTermWhenUnlocked()
    {
        // Arrange
        var value = A.Fake<ITerm>();
        var entity = new Course { Term = A.Fake<ITerm>() };

        // Act
        entity.Term = value;

        // Assert
        entity.Term.ShouldBe(value);
    }

    [Theory]
    [MemberData(nameof(InvalidCourses))]
    public void CompareToHandlesEdgeCases(object other)
    {
        // Arrange
        var course = new Course
        {
            Code = "test.1",
        };

        // Act
        var actual = course.CompareTo(other);

        // Assert
        actual.ShouldBe(1);
    }

    [Theory]
    [InlineData("test.1", "test.2", -1)]
    [InlineData("test.1", "test.1", 0)]
    [InlineData("test.2", "test.1", 1)]
    public void CompareToUsesCodes(string code1, string code2, int expected)
    {
        // Arrange
        var course1 = new Course { Code = code1 };
        var course2 = new Course { Code = code2 };

        // Act
        var actual = course1.CompareTo(course2);

        // Assert
        actual.ShouldBe(expected);
    }

    [Theory]
    [InlineData(123, 123, true)]
    [InlineData(123, 456, false)]
    public void EqualsChecksIds(ulong id1, ulong id2, bool expected)
    {
        // Arrange
        var course1 = new Course { Id = id1 };
        var course2 = new Course { Id = id2 };

        // Act
        var actual = course1.Equals(course2);

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void SettingALockedCanvasThrowsException()
    {
        // Arrange
        var entity = new Course { Canvas = A.Fake<ICanvas>() };
        entity.Lock();

        // Act
        Should.Throw<EntityLockedException>(
            () => entity.Canvas = A.Fake<ICanvas>());
    }

    [Fact]
    public void SettingALockedCodeThrowsException()
    {
        // Arrange
        var entity = new Course { Code = "old code" };
        entity.Lock();

        // Act
        Should.Throw<EntityLockedException>(
            () => entity.Code = "something");
    }

    [Fact]
    public void SettingALockedIdThrowsException()
    {
        // Arrange
        var entity = new Course { Id = 123 };
        entity.Lock();

        // Act
        Should.Throw<EntityLockedException>(
            () => entity.Id = 456);
    }

    [Fact]
    public void SettingALockedNameThrowsException()
    {
        // Arrange
        var entity = new Course { Name = "old name" };
        entity.Lock();

        // Act
        Should.Throw<EntityLockedException>(
            () => entity.Name = "something");
    }

    [Fact]
    public void SettingALockedTermThrowsException()
    {
        // Arrange
        var entity = new Course { Term = A.Fake<ITerm>() };
        entity.Lock();

        // Act
        Should.Throw<EntityLockedException>(
            () => entity.Term = A.Fake<Term>());
    }
}