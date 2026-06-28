using Azure.Identity;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using MicrosoftPlayground.Common.Configurations;

namespace MicrosoftPlayground.Api.Extensions;

public static class ConfigurationManagerExtensions
{
    public static bool AddAzureAppConfigurationProvider(this ConfigurationManager configuration)
    {
        var appConfiguration = configuration
            .GetSection(AzureAppConfigurationConfiguration.SectionName)
            .Get<AzureAppConfigurationConfiguration>() ?? new AzureAppConfigurationConfiguration();

        if (!appConfiguration.Enabled || !HasConnectionDetails(appConfiguration))
        {
            return false;
        }

        configuration.AddAzureAppConfiguration(options =>
        {
            Connect(options, appConfiguration);

            var label = string.IsNullOrWhiteSpace(appConfiguration.Label)
                ? LabelFilter.Null
                : appConfiguration.Label;

            options
                .Select(KeyFilter.Any, label)
                .ConfigureRefresh(refreshOptions => refreshOptions.RegisterAll());

            options.UseFeatureFlags(featureFlagOptions =>
            {
                featureFlagOptions.Select(KeyFilter.Any, label);
                featureFlagOptions.SetRefreshInterval(appConfiguration.GetFeatureFlagCacheExpiration());
            });
        });

        return true;
    }

    private static void Connect(
        AzureAppConfigurationOptions options,
        AzureAppConfigurationConfiguration appConfiguration)
    {
        if (Uri.TryCreate(appConfiguration.Endpoint, UriKind.Absolute, out var endpoint))
        {
            options.Connect(endpoint, new DefaultAzureCredential());
            return;
        }

        options.Connect(appConfiguration.ConnectionString);
    }

    private static bool HasConnectionDetails(AzureAppConfigurationConfiguration appConfiguration)
    {
        return !string.IsNullOrWhiteSpace(appConfiguration.ConnectionString)
            || Uri.TryCreate(appConfiguration.Endpoint, UriKind.Absolute, out _);
    }
}
