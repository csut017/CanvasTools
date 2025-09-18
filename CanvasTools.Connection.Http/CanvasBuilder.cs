using Serilog;

namespace CanvasTools.Connection.Http;

/// <summary>
/// Builder class for generating <see cref="ICanvas"/> instances.
/// </summary>
public sealed class CanvasBuilder
{
    private readonly Configuration _configuration;
    private readonly string _url;

    /// <summary>
    /// Initialise a new <see cref="CanvasBuilder"/> instance.
    /// </summary>
    /// <param name="url">The URL to the Canvas installation.</param>
    public CanvasBuilder(string url)
    {
        _url = url;
        _configuration = new();
    }

    private CanvasBuilder(string url, Configuration configuration)
    {
        _url = url;
        _configuration = configuration;
    }

    /// <summary>
    /// Initialises a new <see cref="ICanvas"/> instance.
    /// </summary>
    /// <returns>The new <see cref="ICanvas"/> instance.</returns>
    public ICanvas Build()
    {
        var logger = _configuration
            .Logger?
            .ForContext<CanvasBuilder>();
        logger
            ?.Information("Building Canvas instance");
        var connection = new RestConnection(_url, _configuration.Token, _configuration.Client)
        {
            Logger = _configuration.Logger?.ForContext<RestConnection>(),
        };
        logger?.Information("Base URL is {url}", connection.Client.BaseAddress);
        var instance = new Canvas
        {
            Connection = connection,
            Logger = _configuration.Logger?.ForContext<Canvas>(),
        };
        return instance;
    }

    /// <summary>
    /// Specifies a <see cref="HttpClient"/> to use.
    /// </summary>
    /// <param name="client">The user token.</param>
    /// <returns>A <see cref="CanvasBuilder"/>.</returns>
    public CanvasBuilder UseHttpClient(HttpClient client)
    {
        var configuration = new Configuration
        {
            Client = client,
            Token = _configuration.Token,
            Logger = _configuration.Logger
        };
        return new CanvasBuilder(
            _url,
            configuration);
    }

    /// <summary>
    /// Specifies the <see cref="ILogger"/> to use.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <returns>A <see cref="CanvasBuilder"/>.</returns>
    public CanvasBuilder UseLogger(ILogger logger)
    {
        var configuration = new Configuration
        {
            Client = _configuration.Client,
            Token = _configuration.Token,
            Logger = logger
        };
        return new CanvasBuilder(
            _url,
            configuration);
    }

    /// <summary>
    /// Specifies a user token to use.
    /// </summary>
    /// <param name="token">The user token.</param>
    /// <returns>A <see cref="CanvasBuilder"/>.</returns>
    public CanvasBuilder UseToken(string token)
    {
        var configuration = new Configuration
        {
            Client = _configuration.Client,
            Token = token,
            Logger = _configuration.Logger
        };
        return new CanvasBuilder(
            _url,
            configuration);
    }

    private class Configuration
    {
        public HttpClient? Client { get; init; }

        public ILogger? Logger { get; init; }

        public string? Token { get; init; }
    }
}