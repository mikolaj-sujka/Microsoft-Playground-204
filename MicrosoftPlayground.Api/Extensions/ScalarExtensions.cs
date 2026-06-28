using MicrosoftPlayground.Common.Configurations;
using Scalar.AspNetCore;

namespace MicrosoftPlayground.Api.Extensions;

public static class ScalarExtensions
{
    public static WebApplication MapScalarDocumentation(this WebApplication app)
    {
        var configuration = app.Configuration
            .GetSection(ScalarConfiguration.SectionName)
            .Get<ScalarConfiguration>() ?? new ScalarConfiguration();

        if (!configuration.Enabled)
        {
            return app;
        }

        if (!app.Environment.IsDevelopment() && !configuration.EnabledOutsideDevelopment)
        {
            return app;
        }

        app.MapOpenApi(configuration.OpenApiRoutePattern);

        app.MapScalarApiReference(configuration.EndpointPrefix, options =>
        {
            options.Title = configuration.Title;
            options.OpenApiRoutePattern = configuration.OpenApiRoutePattern;
            options.ShowSidebar = configuration.ShowSidebar;
            options.DarkMode = configuration.DarkMode;
            options.DocumentDownloadType = configuration.HideDownloadButton
                ? DocumentDownloadType.None
                : DocumentDownloadType.Both;
            options.HideTestRequestButton = configuration.HideTestRequestButton;
            options.DynamicBaseServerUrl = configuration.DynamicBaseServerUrl;
        });

        return app;
    }
}
