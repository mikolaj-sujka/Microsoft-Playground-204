using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MicrosoftPlayground.Api.Extensions;
using MicrosoftPlayground.Api.Telemetry;
using MicrosoftPlayground.Common.Exceptions;

namespace MicrosoftPlayground.Api.Controllers;

[ApiController]
[Route("api/observability")]
public sealed class ObservabilityController(
    ILogger<ObservabilityController> logger,
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration)
    : ControllerBase
{
    [HttpGet("log")]
    public IActionResult WriteStructuredLog([FromQuery] string userId = "demo")
    {
        using var scope = logger.BeginScope(new Dictionary<string, object>
        {
            ["UserId"] = userId,
            ["CorrelationId"] = HttpContext.TraceIdentifier
        });

        using var activity = ApplicationTelemetry.ActivitySource.StartActivity("observability.structured_log");
        activity?.SetTag("app.user_id", userId);
        activity?.SetTag("app.endpoint", HttpContext.Request.Path.Value);

        ApplicationTelemetry.DemoOperations.Add(1, new KeyValuePair<string, object?>("operation", "structured-log"));

        logger.LogInformation(
            "Structured log emitted for {UserId} from {Endpoint}",
            userId,
            HttpContext.Request.Path.Value);

        return Ok(new
        {
            status = "logged",
            userId
        });
    }

    [HttpGet("exception")]
    public IActionResult ThrowException([FromQuery] string type = "bad-request")
    {
        throw type.ToLowerInvariant() switch
        {
            "not-found" => new PlaygroundNotFoundException(
                "Synthetic not found exception.",
                details: "This exception is mapped by GlobalExceptionHandler to HTTP 404."),

            "conflict" => new PlaygroundConflictException(
                "Synthetic conflict exception.",
                details: "This exception is mapped by GlobalExceptionHandler to HTTP 409."),

            "unhandled" => new InvalidOperationException("Synthetic unhandled exception for Application Insights."),

            _ => new PlaygroundBadRequestException(
                "Synthetic application exception mapped to HTTP 400.",
                details: "This exception is mapped by GlobalExceptionHandler to HTTP 400.")
        };
    }

    [HttpGet("dependency")]
    public async Task<IActionResult> SimulateDependency(
        [FromQuery] int statusCode = 200,
        [FromQuery] int delayMs = 250,
        CancellationToken cancellationToken = default)
    {
        statusCode = Math.Clamp(statusCode, 100, 599);
        delayMs = Math.Clamp(delayMs, 0, 10_000);

        var target = $"https://httpstat.us/{statusCode}?sleep={delayMs}";

        using var activity = ApplicationTelemetry.ActivitySource.StartActivity("observability.dependency_simulation");
        activity?.SetTag("dependency.target", target);
        activity?.SetTag("dependency.requested_status_code", statusCode);

        using var scope = logger.BeginScope(new Dictionary<string, object>
        {
            ["DependencyTarget"] = target,
            ["RequestedStatusCode"] = statusCode
        });

        var client = httpClientFactory.CreateClient(ObservabilityExtensions.DependencySimulatorClientName);
        var startedAt = TimeProvider.System.GetTimestamp();
        using var response = await client.GetAsync(target, cancellationToken);
        var elapsed = TimeProvider.System.GetElapsedTime(startedAt);

        ApplicationTelemetry.DemoOperations.Add(1, new KeyValuePair<string, object?>("operation", "dependency"));
        ApplicationTelemetry.DemoDuration.Record(elapsed.TotalMilliseconds, new KeyValuePair<string, object?>("operation", "dependency"));

        logger.LogInformation(
            "Dependency simulation finished with {DependencyStatusCode}",
            (int)response.StatusCode);

        return StatusCode(response.IsSuccessStatusCode ? StatusCodes.Status200OK : StatusCodes.Status502BadGateway, new
        {
            target,
            dependencyStatusCode = (int)response.StatusCode,
            dependencySucceeded = response.IsSuccessStatusCode
        });
    }

    [HttpPost("burst")]
    public async Task<IActionResult> RunTelemetryBurst(
        [FromQuery] int count = 5,
        [FromQuery] string exceptionType = "unhandled",
        CancellationToken cancellationToken = default)
    {
        count = Math.Clamp(count, 1, 20);

        using var activity = ApplicationTelemetry.ActivitySource.StartActivity("observability.failure_burst");
        activity?.SetTag("burst.count", count);
        activity?.SetTag("burst.exception_type", exceptionType);

        using var scope = logger.BeginScope(new Dictionary<string, object>
        {
            ["BurstCount"] = count,
            ["BurstExceptionType"] = exceptionType
        });

        logger.LogInformation("Starting telemetry burst with {BurstCount} iterations.", count);

        var client = httpClientFactory.CreateClient(ObservabilityExtensions.DependencySimulatorClientName);
        var baseUrl = GetSelfTestBaseUrl();
        var results = new List<object>(capacity: count);

        for (var iteration = 1; iteration <= count; iteration++)
        {
            using var iterationActivity = ApplicationTelemetry.ActivitySource.StartActivity("observability.failure_burst.iteration");
            iterationActivity?.SetTag("burst.iteration", iteration);

            var target = $"{baseUrl}/api/observability/exception?type={Uri.EscapeDataString(exceptionType)}";
            using var response = await client.GetAsync(target, cancellationToken);

            ApplicationTelemetry.DemoOperations.Add(1, new KeyValuePair<string, object?>("operation", "burst"));

            if (!response.IsSuccessStatusCode)
            {
                ApplicationTelemetry.DemoFailures.Add(1, new KeyValuePair<string, object?>("operation", "burst"));
                iterationActivity?.SetStatus(ActivityStatusCode.Error, $"HTTP {(int)response.StatusCode}");
                logger.LogWarning(
                    "Burst iteration {Iteration} returned HTTP {StatusCode}",
                    iteration,
                    (int)response.StatusCode);
            }

            results.Add(new
            {
                iteration,
                statusCode = (int)response.StatusCode
            });
        }

        logger.LogInformation("Telemetry burst finished with {BurstCount} iterations.", count);

        return Ok(new
        {
            count,
            exceptionType,
            baseUrl,
            results
        });
    }

    private string GetSelfTestBaseUrl()
    {
        var configuredBaseUrl = configuration["Observability:SelfTestBaseUrl"];

        if (!string.IsNullOrWhiteSpace(configuredBaseUrl))
        {
            return configuredBaseUrl.TrimEnd('/');
        }

        return $"{Request.Scheme}://{Request.Host}".TrimEnd('/');
    }
}
