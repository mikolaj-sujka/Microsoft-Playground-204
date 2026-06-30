using Microsoft.FeatureManagement;
using MicrosoftPlayground.Api.Extensions;
using MicrosoftPlayground.Application.Middleware;
using MicrosoftPlayground.Common.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddApplicationLogging();

var useAzureAppConfiguration = builder.Configuration.AddAzureAppConfigurationProvider();

builder.Services.AddApiServices(builder.Configuration, useAzureAppConfiguration);
builder.Services.AddApplicationObservability(builder.Configuration);

var app = builder.Build();

if (useAzureAppConfiguration)
{
    app.UseAzureAppConfiguration();
}

app.MapScalarDocumentation();

app.UseApplicationExceptionHandling();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddlewareForFeature<TestFeatureMiddleware>(FeatureFlagNames.TestMiddleware);

app.MapApplicationHealthChecks();
app.MapControllers();

app.Run();
