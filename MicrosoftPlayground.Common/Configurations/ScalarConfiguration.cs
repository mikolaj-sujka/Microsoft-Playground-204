namespace MicrosoftPlayground.Common.Configurations;

public sealed class ScalarConfiguration
{
    public const string SectionName = "Scalar";

    public bool Enabled { get; set; } = true;

    public bool EnabledOutsideDevelopment { get; set; }

    public string EndpointPrefix { get; set; } = "/scalar";

    public string OpenApiRoutePattern { get; set; } = "/openapi/{documentName}.json";

    public string Title { get; set; } = "Microsoft Playground API";

    public bool ShowSidebar { get; set; } = true;

    public bool DarkMode { get; set; }

    public bool HideDownloadButton { get; set; }

    public bool HideTestRequestButton { get; set; }

    public bool DynamicBaseServerUrl { get; set; } = true;
}
