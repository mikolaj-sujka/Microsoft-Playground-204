namespace MicrosoftPlayground.Common.Configurations;

public sealed class FeaturesConfiguration
{
    public const string SectionName = "Features";

    public bool BlobStorageEnabled { get; set; }

    public bool UseManagedIdentity { get; set; }

    public HealthCheckFeatureConfiguration HealthCheck { get; set; } = new();
}

public sealed class HealthCheckFeatureConfiguration
{
    public bool Enabled { get; set; }
}
