using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MicrosoftPlayground.Api.Telemetry;
using MicrosoftPlayground.Application.Middleware;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

namespace MicrosoftPlayground.Api.Extensions;

public static class ObservabilityExtensions
{
    public const string DependencySimulatorClientName = "dependency-simulator";

    private const string OpenTelemetryLoggerProviderName =
        "OpenTelemetry.Logs.OpenTelemetryLoggerProvider";

    private const string WindowsEventLogLoggerProviderName =
        "Microsoft.Extensions.Logging.EventLog.EventLogLoggerProvider";

    public static ILoggingBuilder AddApplicationLogging(this ILoggingBuilder logging)
    {
        logging.Configure(options =>
        {
            options.ActivityTrackingOptions =
                ActivityTrackingOptions.TraceId |
                ActivityTrackingOptions.SpanId |
                ActivityTrackingOptions.ParentId;
        });

        logging.Services.Configure<LoggerFilterOptions>(options =>
        {
            options.Rules.Add(new LoggerFilterRule(
                OpenTelemetryLoggerProviderName,
                categoryName: null,
                LogLevel.Information,
                filter: null));

            options.Rules.Add(new LoggerFilterRule(
                WindowsEventLogLoggerProviderName,
                categoryName: null,
                LogLevel.None,
                filter: null));
        });

        return logging;
    }

    public static IServiceCollection AddApplicationObservability(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]
            ?? configuration["ApplicationInsights:ConnectionString"];

        var serviceName = configuration["OpenTelemetry:ServiceName"] ?? "MicrosoftPlayground.Api";
        var serviceVersion = configuration["Application:Version"] ?? "1.0.0";

        services.Configure<OpenTelemetryLoggerOptions>(options =>
        {
            options.IncludeScopes = configuration.GetValue("OpenTelemetry:Logs:IncludeScopes", true);
            options.IncludeFormattedMessage = configuration.GetValue("OpenTelemetry:Logs:IncludeFormattedMessage", true);
            options.ParseStateValues = configuration.GetValue("OpenTelemetry:Logs:ParseStateValues", true);
        });

        var openTelemetry = services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(
                serviceName: serviceName,
                serviceVersion: serviceVersion))
            .WithTracing(tracing => tracing.AddSource(ApplicationTelemetry.ActivitySourceName))
            .WithMetrics(metrics => metrics.AddMeter(ApplicationTelemetry.MeterName));

        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            openTelemetry.UseAzureMonitor(options =>
            {
                options.ConnectionString = connectionString;
                options.EnableLiveMetrics = configuration.GetValue("ApplicationInsights:EnableLiveMetrics", true);
            });
        }

        services.AddHealthChecks();

        services
            .AddHttpClient(DependencySimulatorClientName, client =>
            {
                var timeoutSeconds = configuration.GetValue("Observability:DependencySimulator:TimeoutSeconds", 15);
                client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
            });

        return services;
    }

    public static IApplicationBuilder UseApplicationExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionMiddleware>();
    }

    public static IEndpointRouteBuilder MapApplicationHealthChecks(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHealthChecks("/health", new HealthCheckOptions
        {
            AllowCachingResponses = false
        });

        return endpoints;
    }
}
