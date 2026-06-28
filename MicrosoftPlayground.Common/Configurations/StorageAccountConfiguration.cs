namespace MicrosoftPlayground.Common.Configurations;

public sealed class StorageAccountConfiguration
{
    public const string SectionName = "StorageAccount";

    public string AccountName { get; set; } = string.Empty;

    public string AvatarContainerName { get; set; } = string.Empty;
}
