namespace MicrosoftPlayground.Common.Exceptions;

public sealed class PlaygroundConflictException(
    string message,
    string errorCode = "playground.conflict",
    string? details = null)
    : Exception(message)
{
    public string ErrorCode { get; } = errorCode;

    public string? Details { get; } = details;
}
