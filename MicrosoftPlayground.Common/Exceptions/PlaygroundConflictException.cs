using MicrosoftPlayground.Common.Error;

namespace MicrosoftPlayground.Common.Exceptions;

public sealed class PlaygroundConflictException(
    string message,
    string errorCode = HttpErrorCodes.PlaygroundConflict,
    string? details = null)
    : Exception(message)
{
    public string ErrorCode { get; } = errorCode;

    public string? Details { get; } = details;
}
