using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CanvasTools.Connection.Http.Tests;

[TestSubject(typeof(RestConnection))]
public class RestConnectionTests
{
    [Fact]
    public void ConstructorSetsAcceptHeader()
    {
        // Act
        var connection = new RestConnection("http://127.0.0.1", "test-token");

        // Assert
        connection.Client.DefaultRequestHeaders.Accept.ShouldBe([
            new MediaTypeWithQualityHeaderValue("application/json")
        ]);
    }

    [Fact]
    public void ConstructorSetsAuthorizationHeader()
    {
        // Act
        var connection = new RestConnection("http://127.0.0.1", "test-token");

        // Assert
        connection.Client.DefaultRequestHeaders.Authorization.ShouldNotBeNull();
        connection.Client.ShouldSatisfyAllConditions(
            () => connection.Client.DefaultRequestHeaders.Authorization.Parameter.ShouldBe("test-token"),
            () => connection.Client.DefaultRequestHeaders.Authorization.Scheme.ShouldBe("Bearer")
        );
    }

    [Theory]
    [InlineData("http://127.0.0.1", "http://127.0.0.1")]
    [InlineData("http://127.0.0.1/api", "http://127.0.0.1")]
    [InlineData("http://127.0.0.1/api/v1", "http://127.0.0.1")]
    [InlineData("http://canvas.com", "http://canvas.com")]
    [InlineData("http://canvas.com/hosted", "http://canvas.com/hosted")]
    public void ConstructorSetsUrl(string value, string expected)
    {
        // Act
        var connection = new RestConnection(value);

        // Assert
        var uri = new Uri(expected);
        connection.Client.BaseAddress.ShouldBe(uri);
    }

    [Fact]
    public void ConstructorSkipsAuthorizationHeader()
    {
        // Act
        var connection = new RestConnection("http://127.0.0.1");

        // Assert
        connection.Client.DefaultRequestHeaders.Authorization.ShouldBeNull();
    }

    [Fact]
    public async Task ListHandlesLessThanAPage()
    {
        // Arrange
        var source = Enumerable.Range(1, 5)
            .Select(n => new TestEntity { Id = (ulong)n, Name = $"Test ${n}" })
            .ToList();
        var handler = new StaticJsonHandler(source);
        var client = new HttpClient(handler);
        var connection = new RestConnection(
            "http://127.0.0.1",
            client: client);

        // Act
        var entities = await connection.List<TestEntity>(
            "api/v1/entities/123",
            new Parameters(),
            null,
            TestContext.Current.CancellationToken).ToListAsync(TestContext.Current.CancellationToken);

        // Assert
        entities.ShouldBe(source, new TestEntityEqualityComparer());
    }

    [Fact]
    public async Task ListHandlesMultiplePages()
    {
        // Arrange
        var source = Enumerable.Range(1, 12)
            .Select(n => new TestEntity { Id = (ulong)n, Name = $"Test ${n}" })
            .ToList();
        const string firstUrl = "http://127.0.0.1/api/v1/entities/123";
        const string secondUrl = "http://127.0.0.1/api/v1/entities/123?second";
        var responses = new Dictionary<string, HttpResponseMessage>
        {
            { firstUrl, Helpers.GenerateOkResponse(source.Take(10)).AddHeader("Link", $"<{secondUrl}>; rel=\"next\"") },
            { secondUrl, Helpers.GenerateOkResponse(source.Skip(10)) },
        };
        var handler = new DynamicHandler(
            url => responses.TryGetValue(url, out var response) ? response : throw new Exception("Unexpected url: " + url));
        var client = new HttpClient(handler);
        var connection = new RestConnection(
            "http://127.0.0.1",
            client: client);

        // Act
        var entities = await connection.List<TestEntity>(
            "api/v1/entities/123",
            new Parameters(),
            null,
            TestContext.Current.CancellationToken).ToListAsync(TestContext.Current.CancellationToken);

        // Assert
        entities.ShouldBe(source, new TestEntityEqualityComparer());
        handler.CalledUrls.ShouldBe([firstUrl, secondUrl]);
    }

    [Fact]
    public async Task ListHandlesNoData()
    {
        // Arrange
        var handler = new StaticJsonHandler(Array.Empty<TestEntity>());
        var client = new HttpClient(handler);
        var connection = new RestConnection(
            "http://127.0.0.1",
            client: client);

        // Act
        var entities = await connection.List<TestEntity>(
            "api/v1/entities",
            new Parameters(),
            null,
            TestContext.Current.CancellationToken).ToListAsync(TestContext.Current.CancellationToken);

        // Assert
        entities.ShouldBeEmpty();
        handler.ActualUrl.ShouldBe("http://127.0.0.1/api/v1/entities");
    }

    [Fact]
    public async Task ListHandlesNonSuccessResponseCode()
    {
        // Arrange
        var handler = new StaticJsonHandler(null, HttpStatusCode.NotFound);
        var client = new HttpClient(handler);
        var connection = new RestConnection(
            "http://127.0.0.1",
            client: client);

        // Act and assert
        await Should.ThrowAsync<HttpRequestException>(async () => await connection.List<TestEntity>(
            "api/v1/entities/123",
            new Parameters(),
            null,
            TestContext.Current.CancellationToken).ToListAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ListStopsAtMaxPages()
    {
        // Arrange
        var source = Enumerable.Range(1, 12)
            .Select(n => new TestEntity { Id = (ulong)n, Name = $"Test ${n}" })
            .ToList();
        const string firstUrl = "http://127.0.0.1/api/v1/entities/123";
        const string secondUrl = "http://127.0.0.1/api/v1/entities/123?second";
        var responses = new Dictionary<string, HttpResponseMessage>
        {
            { firstUrl, Helpers.GenerateOkResponse(source.Take(10)).AddHeader("Link", $"<{secondUrl}>; rel=\"next\"") },
            { secondUrl, Helpers.GenerateOkResponse(source.Skip(10)) },
        };
        var handler = new DynamicHandler(
            url => responses.TryGetValue(url, out var response) ? response : throw new Exception("Unexpected url: " + url));
        var client = new HttpClient(handler);
        var connection = new RestConnection(
            "http://127.0.0.1",
            client: client);

        // Act
        var entities = await connection.List<TestEntity>(
            "api/v1/entities/123",
            new Parameters(),
            new ListOptions { MaxPages = 1 },
            TestContext.Current.CancellationToken).ToListAsync(TestContext.Current.CancellationToken);

        // Assert
        entities.ShouldBe(source.Take(10), new TestEntityEqualityComparer());
        handler.CalledUrls.ShouldBe([firstUrl]);
    }

    [Fact]
    public async Task ListStopsWhenProgressHandlerReturnsFalse()
    {
        // Arrange
        var source = Enumerable.Range(1, 12)
            .Select(n => new TestEntity { Id = (ulong)n, Name = $"Test ${n}" })
            .ToList();
        const string firstUrl = "http://127.0.0.1/api/v1/entities/123";
        const string secondUrl = "http://127.0.0.1/api/v1/entities/123?second";
        var responses = new Dictionary<string, HttpResponseMessage>
        {
            { firstUrl, Helpers.GenerateOkResponse(source.Take(10)).AddHeader("Link", $"<{secondUrl}>; rel=\"next\"") },
            { secondUrl, Helpers.GenerateOkResponse(source.Skip(10)) },
        };
        var handler = new DynamicHandler(
            url => responses.TryGetValue(url, out var response) ? response : throw new Exception("Unexpected url: " + url));
        var client = new HttpClient(handler);
        var connection = new RestConnection(
            "http://127.0.0.1",
            client: client);

        // Act
        var entities = await connection.List<TestEntity>(
            "api/v1/entities/123",
            new Parameters(),
            new ListOptions { OnProgressUpdate = _ => true },
            TestContext.Current.CancellationToken).ToListAsync(TestContext.Current.CancellationToken);

        // Assert
        entities.ShouldBe(source.Take(10), new TestEntityEqualityComparer());
        handler.CalledUrls.ShouldBe([firstUrl]);
    }

    [Fact]
    public async Task RetrieveCallsHttpAndDeserializesJson()
    {
        // Arrange
        var source = new TestEntity
        {
            Name = "first",
            Id = 123
        };
        var handler = new StaticJsonHandler(source);
        var client = new HttpClient(handler);
        var connection = new RestConnection(
            "http://127.0.0.1",
            client: client);

        // Act
        var entity = await connection.Retrieve<TestEntity>(
            "api/v1/entities/123",
            new Parameters(),
            TestContext.Current.CancellationToken);

        // Assert
        entity.ShouldNotBeNull();
        entity.ShouldSatisfyAllConditions(
            () => entity.Name.ShouldBe(source.Name),
            () => entity.Id.ShouldBe(source.Id));
        handler.ActualUrl.ShouldBe("http://127.0.0.1/api/v1/entities/123");
    }

    [Fact]
    public async Task RetrieveHandlesNonSuccessResponseCode()
    {
        // Arrange
        var handler = new StaticJsonHandler(null, HttpStatusCode.Unauthorized);
        var client = new HttpClient(handler);
        var connection = new RestConnection(
            "http://127.0.0.1",
            client: client);

        // Act and assert
        await Should.ThrowAsync<HttpRequestException>(async () => await connection.Retrieve<TestEntity>(
            "api/v1/entities/123",
            new Parameters(),
            TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task RetrieveHandlesNotFound()
    {
        // Arrange
        var handler = new StaticJsonHandler(null, HttpStatusCode.NotFound);
        var client = new HttpClient(handler);
        var connection = new RestConnection(
            "http://127.0.0.1",
            client: client);

        // Act
        var entity = await connection.Retrieve<TestEntity>(
            "api/v1/entities/123",
            new Parameters(),
            TestContext.Current.CancellationToken);

        // Assert
        entity.ShouldBeNull();
    }

    [Fact]
    public async Task RetrieveIncludesParameters()
    {
        // Arrange
        var handler = new StaticJsonHandler(null);
        var client = new HttpClient(handler);
        var connection = new RestConnection(
            "http://127.0.0.1",
            client: client);
        var parameters = A.Fake<IParameters>();
        A.CallTo(() => parameters.ToQueryString())
            .Returns("?test=one");

        // Act
        var entity = await connection.Retrieve<TestEntity>(
            "api/v1/entities/123",
            parameters,
            TestContext.Current.CancellationToken);

        // Assert
        entity.ShouldBeNull();
        handler.ActualUrl.ShouldBe("http://127.0.0.1/api/v1/entities/123?test=one");
    }

    public class TestEntity
    {
        public ulong Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class TestEntityEqualityComparer
        : IEqualityComparer<TestEntity>
    {
        public bool Equals(TestEntity x, TestEntity y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null) return false;
            if (y is null) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Id == y.Id && x.Name == y.Name;
        }

        public int GetHashCode(TestEntity obj)
        {
            return HashCode.Combine(obj.Id, obj.Name);
        }
    }
}