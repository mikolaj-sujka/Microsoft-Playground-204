using Azure.Identity;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using MicrosoftPlayground.Common.Configurations;

namespace MicrosoftPlayground.Api.Extensions;

public static class ConfigurationManagerExtensions
{
    public static bool AddAzureAppConfigurationProvider(this ConfigurationManager configuration)
    {
        configuration.ApplyVisualStudioAppConfigurationEndpoint();

        var appConfiguration = configuration
            .GetSection(AzureAppConfigurationConfiguration.SectionName)
            .Get<AzureAppConfigurationConfiguration>() ?? new AzureAppConfigurationConfiguration();

        if (!appConfiguration.Enabled)
        {
            return false;
        }

        var hasEndpoint = Uri.TryCreate(appConfiguration.Endpoint, UriKind.Absolute, out _);

        if (string.IsNullOrWhiteSpace(appConfiguration.ConnectionString) && !hasEndpoint)
        {
            throw new InvalidOperationException(
                "Azure App Configuration is enabled, but no Endpoint, ConnectionString, or AppConfig endpoint was provided.");
        }

        configuration.AddAzureAppConfiguration(options =>
        {
            Connect(options, appConfiguration);

            var label = string.IsNullOrWhiteSpace(appConfiguration.Label)
                ? LabelFilter.Null
                : appConfiguration.Label;

            options
                .Select(KeyFilter.Any, label)
                .ConfigureRefresh(refreshOptions =>
                {
                    if (string.IsNullOrWhiteSpace(appConfiguration.SentinelKey))
                    {
                        refreshOptions.RegisterAll();
                        return;
                    }

                    refreshOptions.Register(appConfiguration.SentinelKey, refreshAll: true);
                });

            options.UseFeatureFlags(featureFlagOptions =>
            {
                featureFlagOptions.Select(KeyFilter.Any, label);
                featureFlagOptions.SetRefreshInterval(appConfiguration.GetFeatureFlagCacheExpiration());
            });
        });

        return true;
    }

    private static void ApplyVisualStudioAppConfigurationEndpoint(this ConfigurationManager configuration)
    {
        var visualStudioEndpoint = configuration[AzureAppConfigurationConfiguration.VisualStudioEndpointKey];

        if (!Uri.TryCreate(visualStudioEndpoint, UriKind.Absolute, out _))
        {
            return;
        }

        // Visual Studio Publish can create an AppConfig setting. Normalize it into
        // our AzureAppConfiguration section so Program.cs keeps one startup path.
        configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            [$"{AzureAppConfigurationConfiguration.SectionName}:Enabled"] = bool.TrueString,
            [$"{AzureAppConfigurationConfiguration.SectionName}:Endpoint"] = visualStudioEndpoint
        });
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
}
