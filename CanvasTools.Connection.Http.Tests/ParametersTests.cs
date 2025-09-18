namespace CanvasTools.Connection.Http.Tests;

[TestSubject(typeof(Parameters))]
public class ParametersTests
{
    [Fact]
    public void ToQueryStringHandlesEmptySet()
    {
        // Arrange
        var parameters = new Parameters();

        // Act
        var queryString = parameters.ToQueryString();

        // Assert
        queryString.ShouldBeEmpty();
    }

    [Fact]
    public void ToQueryStringHandlesMultipleParameters()
    {
        // Arrange
        var parameters = new Parameters
        {
            new Parameter { Name = "first", Value = "one"},
            new Parameter { Name = "second", Value = "two"},
        };

        // Act
        var queryString = parameters.ToQueryString();

        // Assert
        queryString.ShouldBe("?first=one&second=two");
    }

    [Fact]
    public void ToQueryStringHandlesSingleParameter()
    {
        // Arrange
        var parameters = new Parameters
        {
            new Parameter { Name = "first", Value = "one"},
        };

        // Act
        var queryString = parameters.ToQueryString();

        // Assert
        queryString.ShouldBe("?first=one");
    }
}