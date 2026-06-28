namespace MicrosoftPlayground.Common.Configurations;

public static class FeatureFlagNames
{
    public const string BlobStorageEnabled = nameof(BlobStorageEnabled);
    public const string UseManagedIdentity = nameof(UseManagedIdentity);
    public const string HealthCheck = nameof(HealthCheck);
    public const string TestFeatureGate = nameof(TestFeatureGate);
    public const string TestMiddleware = nameof(TestMiddleware);
    public const string TestAnyConditions = nameof(TestAnyConditions);
    public const string TestAllConditions = nameof(TestAllConditions);
}
