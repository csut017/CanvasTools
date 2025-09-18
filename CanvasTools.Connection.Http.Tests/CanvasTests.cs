using System.Threading;
using System.Threading.Tasks;

namespace CanvasTools.Connection.Http.Tests;

[TestSubject(typeof(Canvas))]
public class CanvasTests
{
    [Fact]
    public async Task RetrieveCourseHandlesUnknownCourse()
    {
        // Arrange
        var connection = A.Fake<IConnection>();
        A.CallTo(() => connection.Retrieve<Course>(A<string>.Ignored, A<IParameters>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.FromResult((Course)null));
        var canvas = new Canvas
        {
            Connection = connection,
        };

        // Act
        var actual = await canvas.RetrieveCourse(456);

        // Assert
        A.CallTo(() => connection.Retrieve<Course>("/api/v1/courses/456", A<IParameters>.Ignored, A<CancellationToken>.Ignored))
            .MustHaveHappened();
        actual.ShouldBeNull();
    }

    [Fact]
    public async Task RetrieveCourseReturnsKnownCourse()
    {
        // Arrange
        const string url = "/api/v1/courses/456";
        var connection = A.Fake<IConnection>();
        var course = new Course
        {
            Name = "First course",
            Id = 1,
            Code = "test.1",
        };
        A.CallTo(() => connection.Retrieve<Course>(url, A<IParameters>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(course));
        var canvas = new Canvas
        {
            Connection = connection,
        };

        // Act
        var actual = await canvas.RetrieveCourse(456);

        // Assert
        A.CallTo(() => connection.Retrieve<Course>(url, A<IParameters>.Ignored, A<CancellationToken>.Ignored))
            .MustHaveHappened();
        actual.ShouldBe(course);
        course = (Course)actual;
        course.ShouldSatisfyAllConditions(
            () => course.Canvas.ShouldBe(canvas),
            () => course.IsLocked.ShouldBeTrue(),
            () => actual.Name.ShouldBe("First course"),
            () => actual.Code.ShouldBe("test.1"),
            () => actual.Id.ShouldBe((ulong)1));
    }

    [Fact]
    public async Task RetrieveTermHandlesUnknownTerm()
    {
        // Arrange
        var connection = A.Fake<IConnection>();
        A.CallTo(() => connection.Retrieve<Term>(A<string>.Ignored, A<IParameters>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.FromResult((Term)null));
        var canvas = new Canvas
        {
            Connection = connection,
        };

        // Act
        var actual = await canvas.RetrieveTerm(123, 456);

        // Assert
        A.CallTo(() => connection.Retrieve<Term>("/api/v1/accounts/123/terms/456", A<IParameters>.Ignored, A<CancellationToken>.Ignored))
            .MustHaveHappened();
        actual.ShouldBeNull();
    }

    [Fact]
    public async Task RetrieveTermReturnsKnownTerm()
    {
        // Arrange
        const string url = "/api/v1/accounts/123/terms/456";
        var connection = A.Fake<IConnection>();
        var term = new Term
        {
            Name = "First term",
            Id = 2,
        };
        A.CallTo(() => connection.Retrieve<Term>(url, A<IParameters>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(term));
        var canvas = new Canvas
        {
            Connection = connection,
        };

        // Act
        var actual = await canvas.RetrieveTerm(123, 456);

        // Assert
        A.CallTo(() => connection.Retrieve<Term>(url, A<IParameters>.Ignored, A<CancellationToken>.Ignored))
            .MustHaveHappened();
        actual.ShouldBe(term);
        term = (Term)actual;
        term.ShouldSatisfyAllConditions(
            () => term.Canvas.ShouldBe(canvas),
            () => term.IsLocked.ShouldBeTrue(),
            () => actual.Name.ShouldBe("First term"),
            () => actual.Id.ShouldBe((ulong)2));
    }
}