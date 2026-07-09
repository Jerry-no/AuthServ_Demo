namespace AuthService.Common.ErrorCodes;

public static class AuthErrors
{
    public static readonly ErrorCode InvalidCredentials = new(
        4001,
        "Invalid username or password"
        );

    public static readonly ErrorCode TokenExpired = new(
        4005,
        "Token has expired"
        );

    public static readonly ErrorCode RefreshTokenInvalid = new(
        4006,
        "Refresh token is invalid"
        );

    public static readonly ErrorCode PermissionDenied = new(
        4007,
        "Permission denied"
        );
}