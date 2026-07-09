using AuthService.Common.ErrorCodes;

namespace AuthService.Common.Exceptions;

public sealed class ValidationAppException : AppException
{
    public ValidationAppException(IEnumerable<string> errors)
        : base(CommonErrors.ValidationFailed, errors)
    {
    }
}