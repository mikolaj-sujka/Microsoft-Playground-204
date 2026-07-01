using MicrosoftPlayground.Common.Error;

namespace MicrosoftPlayground.Common.Exceptions;

public sealed class PlaygroundBadRequestException(
    string message,
    string errorCode = HttpErrorCodes.PlaygroundBadRequest,
    string? details = null)
    : Exception(message)
{
    public string ErrorCode { get; } = errorCode;

    public string? Details { get; } = details;
}
