using Microsoft.FeatureManagement;
using MicrosoftPlayground.Api.Extensions;
using MicrosoftPlayground.Application.Middleware;
using MicrosoftPlayground.Common.Configurations;

var builder = WebApplication.CreateBuilder(args);

var useAzureAppConfiguration = builder.Configuration.AddAzureAppConfigurationProvider();

builder.Services.AddApiServices(builder.Configuration, useAzureAppConfiguration);
builder.Services.AddApplicationInisghts(configuration: builder.Configuration);

var app = builder.Build();

if (useAzureAppConfiguration)
{
    app.UseAzureAppConfiguration();
}

app.MapScalarDocumentation();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddlewareForFeature<TestFeatureMiddleware>(FeatureFlagNames.TestMiddleware);

app.MapControllers();

app.Run();
