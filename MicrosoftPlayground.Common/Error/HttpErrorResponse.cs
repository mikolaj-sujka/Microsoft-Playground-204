namespace MicrosoftPlayground.Common.Error;

public sealed class HttpErrorResponse
{
    public int Status { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Detail { get; init; } = string.Empty;

    public string Instance { get; init; } = string.Empty;

    public string ErrorCode { get; init; } = string.Empty;

    public string TraceId { get; init; } = string.Empty;
}
