using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CanvasTools.Connection.Http.Tests;

public class StaticJsonHandler(object data, HttpStatusCode statusCode = HttpStatusCode.OK)
    : HttpMessageHandler
{
    private readonly string _json = JsonSerializer.Serialize(data, JsonSerializerOptions.Web);

    public string ActualUrl { get; private set; } = string.Empty;

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        ActualUrl = request.RequestUri?.ToString() ?? string.Empty;
        var response = new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content = new StringContent(_json, Encoding.UTF8, "application/json"),
        };
        return Task.FromResult(response);
    }
}