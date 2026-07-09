using AuthService.Common.ErrorCodes;

namespace AuthService.Common.Exceptions;

public class AppException : Exception
{
    public ErrorCode ErrorCode { get; }

    public IReadOnlyList<string> Errors { get; }

    public int StatusCode => ErrorCode.Code;

    public AppException(ErrorCode errorCode)
        : base(errorCode.Message)
    {
        ErrorCode = errorCode;
        Errors = Array.Empty<string>();
    }

    public AppException(ErrorCode errorCode, string message)
        : base(message)
    {
        ErrorCode = errorCode with { Message = message };
        Errors = Array.Empty<string>();
    }

    public AppException(ErrorCode errorCode, IEnumerable<string> errors)
        : base(errorCode.Message)
    {
        ErrorCode = errorCode;
        Errors = errors.ToArray();
    }
}