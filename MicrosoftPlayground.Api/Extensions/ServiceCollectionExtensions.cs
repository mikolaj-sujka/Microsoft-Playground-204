using Microsoft.FeatureManagement;
using MicrosoftPlayground.Common.Configurations;

namespace MicrosoftPlayground.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration,
        bool useAzureAppConfiguration)
    {
        services.AddControllers();
        services.AddOpenApi();
        services.AddFeatureManagement();
        services.Configure<ConfigurationFeatureDefinitionProviderOptions>(
            options => options.CustomConfigurationMergingEnabled = true);

        if (useAzureAppConfiguration)
        {
            services.AddAzureAppConfiguration();
        }

        services.AddConfigurationOptions(configuration);

        return services;
    }

    private static IServiceCollection AddConfigurationOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<ApplicationConfiguration>()
            .Bind(configuration.GetSection(ApplicationConfiguration.SectionName))
            .Validate(application => !string.IsNullOrWhiteSpace(application.Name), "Application:Name is required.")
            .Validate(application => !string.IsNullOrWhiteSpace(application.Mode), "Application:Mode is required.")
            .Validate(application => !string.IsNullOrWhiteSpace(application.Region), "Application:Region is required.")
            .Validate(application => !string.IsNullOrWhiteSpace(application.Version), "Application:Version is required.")
            .ValidateOnStart();

        services
            .AddOptions<StorageAccountConfiguration>()
            .Bind(configuration.GetSection(StorageAccountConfiguration.SectionName))
            .Validate(storage => !string.IsNullOrWhiteSpace(storage.AccountName), "StorageAccount:AccountName is required.")
            .Validate(storage => !string.IsNullOrWhiteSpace(storage.AvatarContainerName), "StorageAccount:AvatarContainerName is required.")
            .ValidateOnStart();

        services
            .AddOptions<AzureAppConfigurationConfiguration>()
            .Bind(configuration.GetSection(AzureAppConfigurationConfiguration.SectionName))
            .Validate(
                appConfiguration => !appConfiguration.Enabled
                    || !string.IsNullOrWhiteSpace(appConfiguration.ConnectionString)
                    || Uri.TryCreate(appConfiguration.Endpoint, UriKind.Absolute, out _),
                "AzureAppConfiguration requires Endpoint, ConnectionString, or AppConfig when enabled.")
            .ValidateOnStart();

        services
            .AddOptions<ScalarConfiguration>()
            .Bind(configuration.GetSection(ScalarConfiguration.SectionName))
            .Validate(scalar => !scalar.Enabled || !string.IsNullOrWhiteSpace(scalar.EndpointPrefix), "Scalar:EndpointPrefix is required when Scalar is enabled.")
            .Validate(scalar => !scalar.Enabled || !string.IsNullOrWhiteSpace(scalar.OpenApiRoutePattern), "Scalar:OpenApiRoutePattern is required when Scalar is enabled.")
            .Validate(scalar => !scalar.Enabled || !string.IsNullOrWhiteSpace(scalar.Title), "Scalar:Title is required when Scalar is enabled.")
            .ValidateOnStart();

        return services;
    }
}
