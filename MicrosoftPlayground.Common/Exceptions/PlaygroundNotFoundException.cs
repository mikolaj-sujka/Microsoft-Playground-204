namespace MicrosoftPlayground.Common.Exceptions;

public sealed class PlaygroundNotFoundException(
    string message,
    string errorCode = "playground.not_found",
    string? details = null)
    : Exception(message)
{
    public string ErrorCode { get; } = errorCode;

    public string? Details { get; } = details;
}
