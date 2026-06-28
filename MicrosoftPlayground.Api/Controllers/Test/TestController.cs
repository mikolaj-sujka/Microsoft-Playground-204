using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using MicrosoftPlayground.Application.Middleware;
using MicrosoftPlayground.Common.Configurations;

namespace MicrosoftPlayground.Api.Controllers.Test;

[ApiController]
[Route("api/test")]
// IOptionsSnapshot is scoped per request, so after Azure App Configuration refreshes values,
// the next request can see them. IOptions is a singleton-style value intended for static config.
public class TestController(
    IOptionsSnapshot<ApplicationConfiguration> applicationConfiguration,
    IOptionsSnapshot<StorageAccountConfiguration> storageAccountConfiguration,
    IOptionsSnapshot<AzureAppConfigurationConfiguration> azureAppConfiguration,
    IFeatureManagerSnapshot featureManager)
    : ControllerBase
{
    private static readonly string[] FeatureFlags =
    [
        FeatureFlagNames.BlobStorageEnabled,
        FeatureFlagNames.UseManagedIdentity,
        FeatureFlagNames.HealthCheck,
        FeatureFlagNames.TestFeatureGate,
        FeatureFlagNames.TestMiddleware,
        FeatureFlagNames.TestAnyConditions,
        FeatureFlagNames.TestAllConditions
    ];

    [HttpGet("configuration")]
    public async Task<IActionResult> GetConfiguration()
    {
        return Ok(new
        {
            application = applicationConfiguration.Value,
            storageAccount = storageAccountConfiguration.Value,
            azureAppConfiguration = new
            {
                azureAppConfiguration.Value.Enabled,
                azureAppConfiguration.Value.Endpoint,
                hasConnectionString = !string.IsNullOrWhiteSpace(azureAppConfiguration.Value.ConnectionString),
                azureAppConfiguration.Value.Label,
                azureAppConfiguration.Value.FeatureFlagCacheExpirationSeconds
            },
            featureFlags = await GetFeatureFlagStates(),
            middlewareEnabledForRequest = HttpContext.Items.ContainsKey(TestFeatureMiddleware.HttpContextItemKey)
        });
    }

    [HttpGet("feature-gate")]
    [FeatureGate(FeatureFlagNames.TestFeatureGate)]
    public IActionResult GetFeatureGate()
    {
        return Ok(new
        {
            feature = FeatureFlagNames.TestFeatureGate,
            status = "FeatureGate allowed this action."
        });
    }

    [HttpGet("middleware")]
    public IActionResult GetMiddlewareState()
    {
        return Ok(new
        {
            feature = FeatureFlagNames.TestMiddleware,
            middlewareEnabledForRequest = HttpContext.Items.ContainsKey(TestFeatureMiddleware.HttpContextItemKey),
            responseHeader = TestFeatureMiddleware.ResponseHeaderName
        });
    }

    private async Task<IDictionary<string, bool>> GetFeatureFlagStates()
    {
        var states = new Dictionary<string, bool>();

        foreach (var featureFlag in FeatureFlags)
        {
            states[featureFlag] = await featureManager.IsEnabledAsync(featureFlag);
        }

        return states;
    }
}
