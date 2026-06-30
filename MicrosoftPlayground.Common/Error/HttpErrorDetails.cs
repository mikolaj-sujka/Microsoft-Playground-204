namespace MicrosoftPlayground.Common.Error;

public sealed record HttpErrorDetails(
    Exception Exception,
    int StatusCode,
    string Title,
    string Detail,
    string ErrorCode,
    string TraceId,
    string Path)
{
    public bool IsServerError => StatusCode >= 500;
}
