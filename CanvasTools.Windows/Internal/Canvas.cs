using CanvasTools.Connection.Http;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace CanvasTools.Windows.Internal;

internal static class Canvas
{
    /// <summary>
    /// Initialize the Canvas client.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the client to.</param>
    /// <param name="url">The URL for the Canvas REST API.</param>
    /// <param name="token">The user token to use.</param>
    public static void InitializeCanvas(this IServiceCollection services, string url, string token)
    {
        Guard.IsNotNull(services);
        Guard.IsNotNullOrWhiteSpace(url);
        Guard.IsNotNullOrWhiteSpace(token);
        services.AddSingleton(
            sp =>
            {
                var builder = new CanvasBuilder(url)
                    .UseToken(token)
                    .UseLogger(sp.GetRequiredService<ILogger>());
                return builder.Build();
            });
    }
}