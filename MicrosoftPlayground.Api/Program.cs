using Microsoft.Azure.AppConfiguration.AspNetCore;
using Microsoft.FeatureManagement;
using MicrosoftPlayground.Api.Extensions;
using MicrosoftPlayground.Application.Middleware;
using MicrosoftPlayground.Common.Configurations;

var builder = WebApplication.CreateBuilder(args);

var useAzureAppConfiguration = builder.Configuration.AddAzureAppConfigurationProvider();

builder.Services.AddApiServices(builder.Configuration, useAzureAppConfiguration);

var app = builder.Build();

if (useAzureAppConfiguration)
{
    app.UseAzureAppConfiguration();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddlewareForFeature<TestFeatureMiddleware>(FeatureFlagNames.TestMiddleware);

app.MapControllers();

app.Run();
