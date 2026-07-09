namespace AuthService.Common.ErrorCodes;

public static class CommonErrors
{
    public static readonly ErrorCode Success = new(
        StatusCodes.Status200OK,
        "Success"
        );

    public static readonly ErrorCode ValidationFailed = new(
        StatusCodes.Status400BadRequest,
        "Validation failed"
        );

    public static readonly ErrorCode BadRequest = new(
        StatusCodes.Status400BadRequest,
        "Bad request"
        );

    public static readonly ErrorCode Unauthorized = new(
        StatusCodes.Status401Unauthorized,
        "Unauthorized"
        );

    public static readonly ErrorCode Forbidden = new(
        StatusCodes.Status403Forbidden,
        "Forbidden"
        );

    public static readonly ErrorCode NotFound = new(
        StatusCodes.Status404NotFound,
        "Resource not found"
        );

    public static readonly ErrorCode Conflict = new(
        StatusCodes.Status409Conflict,
        "Conflict"
        );
}