using System.Net.Http;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.InMemory;
using Serilog.Sinks.InMemory.Assertions;

namespace CanvasTools.Connection.Http.Tests;

[TestSubject(typeof(CanvasBuilder))]
public class CanvasBuilderTests
{
    [Fact]
    public void BuildGeneratesACanvasInstance()
    {
        // Arrange
        var builder = new CanvasBuilder("http://127.0.0.1");

        // Act
        var canvas = builder.Build();

        // Assert
        canvas.ShouldNotBeNull();
    }

    [Fact]
    public void BuildGeneratesHttpClient()
    {
        // Arrange
        var builder = new CanvasBuilder("http://127.0.0.1");

        // Act
        var canvas = builder.Build();

        // Assert
        var connection = canvas.Connection as RestConnection;
        connection.ShouldNotBeNull();
        connection.Client.ShouldNotBeNull();
    }

    [Fact]
    public void UseHttpClientSetsClient()
    {
        // Arrange
        var builder = new CanvasBuilder("http://127.0.0.1");
        var client = new HttpClient();

        // Act
        builder = builder.UseHttpClient(client);
        var canvas = builder.Build();

        // Assert
        var connection = canvas.Connection as RestConnection;
        connection.ShouldNotBeNull();
        connection.Client.ShouldBe(client);
    }

    [Fact]
    public void UseLoggerAssociatesLogger()
    {
        // Arrange
        var logger = new LoggerConfiguration()
            .WriteTo.InMemory()
            .CreateLogger();
        var builder = new CanvasBuilder("http://127.0.0.1");

        // Act
        builder = builder.UseLogger(logger);
        var canvas = builder.Build();

        // Assert
        canvas.ShouldNotBeNull();
        InMemorySink.Instance
            .Should()
            .HaveMessage("Building Canvas instance")
            .WithProperty(Constants.SourceContextPropertyName);
    }

    [Fact]
    public void UseTokenSetsAuthorizationHeader()
    {
        // Arrange
        var builder = new CanvasBuilder("http://127.0.0.1");
        var client = new HttpClient();

        // Act
        builder = builder.UseToken("test-token")
            .UseHttpClient(client);
        var canvas = builder.Build();

        // Assert
        var connection = canvas.Connection as RestConnection;
        connection.ShouldNotBeNull();
        connection.Client.ShouldBe(client);
        client.DefaultRequestHeaders.Authorization.ShouldNotBeNull();
        client.ShouldSatisfyAllConditions(
            () => client.DefaultRequestHeaders.Authorization.Parameter.ShouldBe("test-token"),
            () => client.DefaultRequestHeaders.Authorization.Scheme.ShouldBe("Bearer")
        );
    }
}