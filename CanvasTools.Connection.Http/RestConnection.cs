using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Serilog;

namespace CanvasTools.Connection.Http;

/// <summary>
/// A connection to a Canvas installation that uses HTTP REST calls.
/// </summary>
public class RestConnection
    : IConnection
{
    private static readonly JsonSerializerOptions SerializerOptions = JsonSerializerOptions.Web;

    /// <summary>
    /// Initialise a new <see cref="RestConnection"/> instance.
    /// </summary>
    /// <param name="url">The base URL to the Canvas installation.</param>
    /// <param name="token">The default user token for authorisation.</param>
    /// <param name="client">The underlying <see cref="HttpClient"/> to use.</param>
    public RestConnection(string url, string? token = null, HttpClient? client = null)
    {
        Client = client ?? new HttpClient();
        Client.BaseAddress = GenerateCleanUri(url);

        if (!string.IsNullOrEmpty(token))
        {
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        Client.DefaultRequestHeaders.Accept.Clear();
        Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    /// <summary>
    /// The underlying <see cref="HttpClient"/>.
    /// </summary>
    public HttpClient Client { get; }

    /// <summary>
    /// The logger to use for recording actions.
    /// </summary>
    public ILogger? Logger { get; init; }

    /// <summary>
    /// Retrieves a list of entities fom the Canvas installation.
    /// </summary>
    /// <typeparam name="TItem">The type of item to retrieve.</typeparam>
    /// <param name="url">The URL to use.</param>
    /// <param name="parameters">The parameters to pass.</param>
    /// <param name="options">The options for retrieving the data.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to use.</param>
    /// <returns>An <see cref="IQueryable{TItem}"/> containing the entities from Canvas.</returns>
    public async IAsyncEnumerable<TItem> List<TItem>(string url, IParameters parameters, ListOptions? options = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        where TItem : class
    {
        var fullUrl = url + parameters.ToQueryString();
        Logger?.Debug("Listing {type} entities from {url}", typeof(TItem).Name, fullUrl);
        var pageNumber = 0;
        var itemCount = 0;
        var cancel = false;
        var maxPage = options?.MaxPages ?? int.MaxValue;
        while (!cancel && !string.IsNullOrEmpty(fullUrl) && pageNumber < maxPage)
        {
            Logger?.Debug("Retrieving page {page} of {type} entities from {url}", pageNumber, typeof(TItem).Name, fullUrl);
            var response = await Client.GetAsync(fullUrl, cancellationToken);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStreamAsync(cancellationToken);

            var items = await JsonSerializer.DeserializeAsync<TItem[]>(
                json,
                options: SerializerOptions,
                cancellationToken: cancellationToken) ?? [];
            foreach (var item in items)
            {
                yield return item;
                itemCount++;
            }

            fullUrl = GetNextLink(response.Headers);
            pageNumber++;
            cancel = options?.OnProgressUpdate?.Invoke(new ProgressUpdate(itemCount, pageNumber)) ?? false;
        }
    }

    /// <summary>
    /// Retrieves an entity from the Canvas installation.
    /// </summary>
    /// <typeparam name="TItem">The type of item to retrieve.</typeparam>
    /// <param name="url">The URL to use.</param>
    /// <param name="parameters">The parameters to pass.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to use.</param>
    /// <returns>A <see cref="TItem"/> instance if found; <see langword="null" /> otherwise.</returns>
    public async Task<TItem?> Retrieve<TItem>(string url, IParameters parameters, CancellationToken cancellationToken = default)
        where TItem : class
    {
        var fullUrl = url + parameters.ToQueryString();
        Logger?.Debug("Retrieving {type} entity from {url}", typeof(TItem).Name, fullUrl);
        var response = await Client.GetAsync(fullUrl, cancellationToken);
        if (response.StatusCode != HttpStatusCode.NotFound) response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStreamAsync(cancellationToken);

        var item = await JsonSerializer.DeserializeAsync<TItem>(
            json,
            options: SerializerOptions,
            cancellationToken: cancellationToken);
        return item;
    }

    /// <summary>
    /// Generate a <see cref="Uri"/> without the api or version at the end of it.
    /// </summary>
    /// <param name="url">A <see langword="string" /> containing the raw URL.</param>
    /// <returns>A cleaned <see cref="Uri"/> instance.</returns>
    private static Uri GenerateCleanUri(string url)
    {
        var uri = new Uri(url);

        var length = uri.Segments.Length - 1;
        if (string.Equals(uri.Segments[length], "v1", StringComparison.InvariantCultureIgnoreCase)
            || string.Equals(uri.Segments[length], "v1/", StringComparison.InvariantCultureIgnoreCase)) length--;
        if (string.Equals(uri.Segments[length], "api", StringComparison.InvariantCultureIgnoreCase)
            || string.Equals(uri.Segments[length], "api/", StringComparison.InvariantCultureIgnoreCase)) length--;

        var builder = new UriBuilder(uri)
        {
            Path = string.Concat(uri.Segments.Take(length + 1))
        };

        return builder.Uri;
    }

    private static string? GetNextLink(HttpResponseHeaders headers)
    {
        if (!headers.Contains("Link")) return null;
        var header = string.Join(",", headers.NonValidated["Link"]);
        return (from link
                in header.Split(',', StringSplitOptions.RemoveEmptyEntries)
                let colonPos = link.IndexOf(';')
                let type = link[(colonPos + 1)..].Trim()
                where type == "rel=\"next\""
                select link[..colonPos].Trim() into url
                select url[1..^1]).FirstOrDefault();
    }
}