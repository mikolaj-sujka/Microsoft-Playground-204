using Microsoft.AspNetCore.Http;

namespace MicrosoftPlayground.Application.Middleware;

public sealed class TestFeatureMiddleware(RequestDelegate next)
{
    public const string HttpContextItemKey = "TestFeatureMiddleware.Enabled";
    public const string ResponseHeaderName = "X-Test-Feature-Middleware";

    public async Task InvokeAsync(HttpContext context)
    {
        context.Items[HttpContextItemKey] = true;
        context.Response.Headers[ResponseHeaderName] = "enabled";

        await next(context);
    }
}
