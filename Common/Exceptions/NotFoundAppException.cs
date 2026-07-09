using AuthService.Common.ErrorCodes;

namespace AuthService.Common.Exceptions;

public sealed class NotFoundAppException : AppException
{
    public NotFoundAppException()
        : base(CommonErrors.NotFound)
    {
    }

    public NotFoundAppException(string message)
        : base(CommonErrors.NotFound, message)
    {
    }
}