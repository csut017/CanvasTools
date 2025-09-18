using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CanvasTools.Connection.Http.Tests;

[TestSubject(typeof(User))]
public class UserTests
{
    public static TheoryData<object> InvalidUsers => [(object)null, (object)"garbage", new object()];

    [Fact]
    public void CanSetCanvasWhenUnlocked()
    {
        // Arrange
        var value = A.Fake<ICanvas>();
        var entity = new User { Canvas = null };

        // Act
        entity.Canvas = value;

        // Assert
        entity.Canvas.ShouldBe(value);
    }

    [Fact]
    public void CanSetIdWhenUnlocked()
    {
        // Arrange
        ulong value = 123;
        var entity = new User { Id = 456 };

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
        var entity = new User { Name = "old name" };

        // Act
        entity.Name = value;

        // Assert
        entity.Name.ShouldBe(value);
    }

    [Theory]
    [MemberData(nameof(InvalidUsers))]
    public void CompareToHandlesEdgeCases(object other)
    {
        // Arrange
        var user = new User
        {
            Name = "Bob Smith",
        };

        // Act
        var actual = user.CompareTo(other);

        // Assert
        actual.ShouldBe(1);
    }

    [Theory]
    [InlineData("Bob", "Bill", 6)]
    [InlineData("Bob", "Bob", 0)]
    [InlineData("Bill", "Bob", -6)]
    public void CompareToUsesWhenStarts(string name1, string name2, int expected)
    {
        // Arrange
        var user1 = new User { Name = name1 };
        var user2 = new User { Name = name2 };

        // Act
        var actual = user1.CompareTo(user2);

        // Assert
        actual.ShouldBe(expected);
    }

    [Theory]
    [InlineData(123, 123, true)]
    [InlineData(123, 456, false)]
    public void EqualsChecksIds(ulong id1, ulong id2, bool expected)
    {
        // Arrange
        var user1 = new User { Id = id1 };
        var user2 = new User { Id = id2 };

        // Act
        var actual = user1.Equals(user2);

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public async Task RetrieveCoursesCallsConnection()
    {
        // Arrange
        const string url = "api/v1/users/1/courses";
        var connection = A.Fake<IConnection>();
        var canvas = new Canvas
        {
            Connection = connection,
        };
        var user = new User
        {
            Id = 1,
            Canvas = canvas
        };

        // Act
        var actual = await user
            .ListCourses(TestContext.Current.CancellationToken)
            .ToListAsync(TestContext.Current.CancellationToken);

        // Assert
        A.CallTo(() => connection.List<Course>(url, A<IParameters>.Ignored, A<ListOptions>.Ignored, A<CancellationToken>.Ignored))
            .MustHaveHappened();
    }

    [Fact]
    public void SettingALockedCanvasThrowsException()
    {
        // Arrange
        var entity = new User { Canvas = A.Fake<ICanvas>() };
        entity.Lock();

        // Act
        Should.Throw<EntityLockedException>(
            () => entity.Canvas = A.Fake<ICanvas>());
    }

    [Fact]
    public void SettingALockedIdThrowsException()
    {
        // Arrange
        var entity = new User { Id = 123 };
        entity.Lock();

        // Act
        Should.Throw<EntityLockedException>(
            () => entity.Id = 456);
    }

    [Fact]
    public void SettingALockedNameThrowsException()
    {
        // Arrange
        var entity = new User { Name = "old name" };
        entity.Lock();

        // Act
        Should.Throw<EntityLockedException>(
            () => entity.Name = "something");
    }
}