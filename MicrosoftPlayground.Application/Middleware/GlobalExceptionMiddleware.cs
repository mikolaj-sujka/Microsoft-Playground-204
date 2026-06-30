using System.Diagnostics;
using System.Runtime.ExceptionServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MicrosoftPlayground.Common.Error;
using MicrosoftPlayground.Common.Exceptions;

namespace MicrosoftPlayground.Application.Middleware;

public sealed class GlobalExceptionMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            var errorDetails = CreateErrorDetails(context, exception);
            await WriteErrorResponseAsync(context, errorDetails);
        }
    }

    private async Task WriteErrorResponseAsync(
        HttpContext context,
        HttpErrorDetails errorDetails)
    {
        if (context.Response.HasStarted)
        {
            logger.LogError(errorDetails.Exception, "An exception was thrown after the response had already started.");
            ExceptionDispatchInfo.Capture(errorDetails.Exception).Throw();
        }

        var logLevel = errorDetails.IsServerError ? LogLevel.Error : LogLevel.Warning;

        Activity.Current?.SetTag("error.type", errorDetails.Exception.GetType().FullName);
        Activity.Current?.SetTag("error.code", errorDetails.ErrorCode);
        Activity.Current?.SetTag("http.error.status_code", errorDetails.StatusCode);
        Activity.Current?.AddException(errorDetails.Exception);
        Activity.Current?.SetStatus(ActivityStatusCode.Error, errorDetails.Detail);

        logger.Log(
            logLevel,
            errorDetails.Exception,
            "Handled exception {ErrorCode} as HTTP {StatusCode} for {RequestPath}",
            errorDetails.ErrorCode,
            errorDetails.StatusCode,
            errorDetails.Path);

        context.Response.Clear();
        context.Response.StatusCode = errorDetails.StatusCode;
        context.Response.ContentType = "application/problem+json";

        var response = new HttpErrorResponse
        {
            Status = errorDetails.StatusCode,
            Title = errorDetails.Title,
            Detail = errorDetails.Detail,
            Instance = errorDetails.Path,
            ErrorCode = errorDetails.ErrorCode,
            TraceId = errorDetails.TraceId
        };

        await context.Response.WriteAsJsonAsync(response);
    }

    private static HttpErrorDetails CreateErrorDetails(HttpContext context, Exception exception)
    {
        var traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;
        var path = context.Request.Path.Value ?? string.Empty;

        return exception switch
        {
            PlaygroundBadRequestException badRequest => new HttpErrorDetails(
                badRequest,
                StatusCodes.Status400BadRequest,
                "Bad request",
                badRequest.Details ?? badRequest.Message,
                badRequest.ErrorCode,
                traceId,
                path),

            PlaygroundNotFoundException notFound => new HttpErrorDetails(
                notFound,
                StatusCodes.Status404NotFound,
                "Resource not found",
                notFound.Details ?? notFound.Message,
                notFound.ErrorCode,
                traceId,
                path),

            PlaygroundConflictException conflict => new HttpErrorDetails(
                conflict,
                StatusCodes.Status409Conflict,
                "Conflict",
                conflict.Details ?? conflict.Message,
                conflict.ErrorCode,
                traceId,
                path),

            BadHttpRequestException badHttpRequest => new HttpErrorDetails(
                badHttpRequest,
                StatusCodes.Status400BadRequest,
                "Bad request",
                badHttpRequest.Message,
                "http.bad_request",
                traceId,
                path),

            _ => new HttpErrorDetails(
                exception,
                StatusCodes.Status500InternalServerError,
                "Unexpected server error",
                exception.Message,
                "http.unhandled_exception",
                traceId,
                path)
        };
    }
}
