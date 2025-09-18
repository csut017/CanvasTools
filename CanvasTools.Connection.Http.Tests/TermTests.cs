using System;

namespace CanvasTools.Connection.Http.Tests;

[TestSubject(typeof(Term))]
public class TermTests
{
    public static TheoryData<object> InvalidTerms => [(object)null, (object)"garbage", new object()];

    [Fact]
    public void CanSetCanvasWhenUnlocked()
    {
        // Arrange
        var value = A.Fake<ICanvas>();
        var entity = new Term { Canvas = null };

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
        var entity = new Term { Id = 456 };

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
        var entity = new Term { Name = "old name" };

        // Act
        entity.Name = value;

        // Assert
        entity.Name.ShouldBe(value);
    }

    [Fact]
    public void CanSetWhenEndsWhenUnlocked()
    {
        // Arrange
        var value = new DateTime(2000, 1, 1);
        var entity = new Term { WhenEnds = new DateTime(2001, 1, 1) };

        // Act
        entity.WhenEnds = value;

        // Assert
        entity.WhenEnds.ShouldBe(value);
    }

    [Fact]
    public void CanSetWhenStartsWhenUnlocked()
    {
        // Arrange
        var value = new DateTime(2000, 1, 1);
        var entity = new Term { WhenStarts = new DateTime(2001, 1, 1) };

        // Act
        entity.WhenStarts = value;

        // Assert
        entity.WhenStarts.ShouldBe(value);
    }

    [Theory]
    [MemberData(nameof(InvalidTerms))]
    public void CompareToHandlesEdgeCases(object other)
    {
        // Arrange
        var term = new Term
        {
            WhenStarts = new DateTime(2000, 1, 1),
        };

        // Act
        var actual = term.CompareTo(other);

        // Assert
        actual.ShouldBe(1);
    }

    [Theory]
    [InlineData(1, 10, -1)]
    [InlineData(10, 10, 0)]
    [InlineData(10, 1, 1)]
    public void CompareToUsesWhenStarts(int day1, int day2, int expected)
    {
        // Arrange
        var term1 = new Term { WhenStarts = new DateTime(2000, 1, day1) };
        var term2 = new Term { WhenStarts = new DateTime(2000, 1, day2) };

        // Act
        var actual = term1.CompareTo(term2);

        // Assert
        actual.ShouldBe(expected);
    }

    [Theory]
    [InlineData(123, 123, true)]
    [InlineData(123, 456, false)]
    public void EqualsChecksIds(ulong id1, ulong id2, bool expected)
    {
        // Arrange
        var term1 = new Term { Id = id1 };
        var term2 = new Term { Id = id2 };

        // Act
        var actual = term1.Equals(term2);

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void SettingALockedCanvasThrowsException()
    {
        // Arrange
        var entity = new Term { Canvas = A.Fake<ICanvas>() };
        entity.Lock();

        // Act
        Should.Throw<EntityLockedException>(
            () => entity.Canvas = A.Fake<ICanvas>());
    }

    [Fact]
    public void SettingALockedIdThrowsException()
    {
        // Arrange
        var entity = new Term { Id = 123 };
        entity.Lock();

        // Act
        Should.Throw<EntityLockedException>(
            () => entity.Id = 456);
    }

    [Fact]
    public void SettingALockedNameThrowsException()
    {
        // Arrange
        var entity = new Term { Name = "old name" };
        entity.Lock();

        // Act
        Should.Throw<EntityLockedException>(
            () => entity.Name = "something");
    }

    [Fact]
    public void SettingALockedWhenEndsThrowsException()
    {
        // Arrange
        var entity = new Term { WhenEnds = new DateTime(2000, 1, 1) };
        entity.Lock();

        // Act
        Should.Throw<EntityLockedException>(
            () => entity.WhenEnds = new DateTime(2001, 1, 1));
    }

    [Fact]
    public void SettingALockedWhenStartsThrowsException()
    {
        // Arrange
        var entity = new Term { WhenStarts = new DateTime(2000, 1, 1) };
        entity.Lock();

        // Act
        Should.Throw<EntityLockedException>(
            () => entity.WhenStarts = new DateTime(2001, 1, 1));
    }
}