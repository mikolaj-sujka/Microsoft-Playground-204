namespace MicrosoftPlayground.Common.Exceptions;

public sealed class PlaygroundBadRequestException(
    string message,
    string errorCode = "playground.bad_request",
    string? details = null)
    : Exception(message)
{
    public string ErrorCode { get; } = errorCode;

    public string? Details { get; } = details;
}
