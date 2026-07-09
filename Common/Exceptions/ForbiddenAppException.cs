using AuthService.Common.ErrorCodes;

namespace AuthService.Common.Exceptions;

public sealed class ForbiddenAppException : AppException
{
    public ForbiddenAppException()
        : base(CommonErrors.Forbidden)
    {
    }

    public ForbiddenAppException(string message)
        : base(CommonErrors.Forbidden, message)
    {
    }
}