using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using MicrosoftPlayground.Common.Configurations;

namespace MicrosoftPlayground.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
// IOptionsSnapshot is scoped per request, so after Azure App Configuration refreshes values,
// the next request can see them. IOptions is a singleton-style value intended for static config.
public class HealthController(
    IOptionsSnapshot<ApplicationConfiguration> applicationConfiguration,
    IFeatureManagerSnapshot featureManager)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetHealth()
    {
        if (!await featureManager.IsEnabledAsync(FeatureFlagNames.HealthCheck))
        {
            return NotFound();
        }

        var application = applicationConfiguration.Value;

        return Ok(new
        {
            status = "Healthy",
            application = application.Name,
            mode = application.Mode,
            region = application.Region,
            version = application.Version
        });
    }
}
