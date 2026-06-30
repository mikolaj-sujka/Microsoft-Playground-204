using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace MicrosoftPlayground.Api.Telemetry;

public static class ApplicationTelemetry
{
    public const string ActivitySourceName = "MicrosoftPlayground.Api";
    public const string MeterName = "MicrosoftPlayground.Api";

    public static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    private static readonly Meter Meter = new(MeterName);

    public static readonly Counter<long> DemoOperations = Meter.CreateCounter<long>(
        "microsoft_playground.demo.operations",
        unit: "{operation}",
        description: "Number of demo observability operations.");

    public static readonly Counter<long> DemoFailures = Meter.CreateCounter<long>(
        "microsoft_playground.demo.failures",
        unit: "{failure}",
        description: "Number of demo observability failures.");

    public static readonly Histogram<double> DemoDuration = Meter.CreateHistogram<double>(
        "microsoft_playground.demo.duration",
        unit: "ms",
        description: "Duration of demo observability operations.");
}
