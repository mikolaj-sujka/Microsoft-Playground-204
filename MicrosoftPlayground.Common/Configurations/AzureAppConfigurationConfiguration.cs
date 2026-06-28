namespace MicrosoftPlayground.Common.Configurations;

public sealed class AzureAppConfigurationConfiguration
{
    public const string SectionName = "AzureAppConfiguration";
    public const string VisualStudioEndpointKey = "AppConfig";

    public bool Enabled { get; set; }

    public string Endpoint { get; set; } = string.Empty;

    public string ConnectionString { get; set; } = string.Empty;

    public string Label { get; set; } = string.Empty;

    public string SentinelKey { get; set; } = "TestApp:Settings:Sentinel";

    public int FeatureFlagCacheExpirationSeconds { get; set; } = 30;

    public TimeSpan GetFeatureFlagCacheExpiration()
    {
        return TimeSpan.FromSeconds(Math.Max(1, FeatureFlagCacheExpirationSeconds));
    }
}
