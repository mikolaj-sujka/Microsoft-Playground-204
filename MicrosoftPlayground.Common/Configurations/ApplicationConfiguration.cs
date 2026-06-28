namespace MicrosoftPlayground.Common.Configurations;

public sealed class ApplicationConfiguration
{
    public const string SectionName = "Application";

    public string Name { get; set; } = string.Empty;

    public string Mode { get; set; } = string.Empty;

    public string Region { get; set; } = string.Empty;

    public string Version { get; set; } = string.Empty;
}
