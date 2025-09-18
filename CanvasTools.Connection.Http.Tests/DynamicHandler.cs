using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CanvasTools.Connection.Http.Tests;

public class DynamicHandler(Func<string, HttpResponseMessage> handler)
    : HttpMessageHandler
{
    public List<string> CalledUrls { get; } = [];

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var url = request.RequestUri?.ToString() ?? string.Empty;
        CalledUrls.Add(url);
        var response = handler(url);
        return Task.FromResult(response);
    }
}