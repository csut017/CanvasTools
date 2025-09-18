using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace CanvasTools.Connection.Http.Tests;

public static class Helpers
{
    public static HttpResponseMessage AddHeader(this HttpResponseMessage message, string key, string value)
    {
        message.Headers.Add(key, value);
        return message;
    }

    public static HttpResponseMessage GenerateOkResponse(object source)
    {
        var json = JsonSerializer.Serialize(source, JsonSerializerOptions.Web);
        return new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
        };
    }
}