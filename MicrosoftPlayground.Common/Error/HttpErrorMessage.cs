namespace MicrosoftPlayground.Common.Error;

public static class HttpErrorMessage
{
    public const string BadRequestTitle = "Bad request";
    public const string ResourceNotFoundTitle = "Resource not found";
    public const string ConflictTitle = "Conflict";
    public const string UnexpectedServerErrorTitle = "Unexpected server error";

    public const string SyntheticBadRequest = "Synthetic application exception mapped to HTTP 400.";
    public const string SyntheticNotFound = "Synthetic not found exception.";
    public const string SyntheticConflict = "Synthetic conflict exception.";
    public const string SyntheticUnhandledException = "Synthetic unhandled exception for Application Insights.";

    public const string MappedToHttp400 = "This exception is mapped by GlobalExceptionHandler to HTTP 400.";
    public const string MappedToHttp404 = "This exception is mapped by GlobalExceptionHandler to HTTP 404.";
    public const string MappedToHttp409 = "This exception is mapped by GlobalExceptionHandler to HTTP 409.";

    public const string ResponseAlreadyStarted =
        "An exception was thrown after the response had already started.";

    public const string HandledExceptionTemplate =
        "Handled exception {ErrorCode} as HTTP {StatusCode} for {RequestPath}";
}
