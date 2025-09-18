using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace CanvasTools.Windows.Internal;

internal static class AppSettings
{
    public static AppSettings<T> New<T>(string configPath)
        where T : class
    {
        var settings = new AppSettings<T>();
        settings.Initialise(configPath);
        return settings;
    }
}

internal class AppSettings<TType>
    where TType : class
{
    private IConfiguration? _config;

    private bool _isInitialized;

    public string CanvasToken => GetSetting("canvas:token");

    public string CanvasUrl => GetSetting("canvas:url");

    public string GetSetting(string path)
    {
        if (!_isInitialized) throw new InvalidOperationException("Cannot get a setting before the settings have been initialized");

        var section = this._config!.GetSection(path);
        return (section.Exists()
            ? section.Value
            : string.Empty) ?? string.Empty;
    }

    public void Initialise(string configPath)
    {
        Guard.IsNotNullOrWhiteSpace(configPath);
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile(configPath);
        builder.AddUserSecrets<TType>();
        _config = builder.Build();
        _isInitialized = true;
    }
}