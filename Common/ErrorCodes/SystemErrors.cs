namespace AuthService.Common.ErrorCodes;

public static class SystemErrors
{
    public static readonly ErrorCode InternalServerError = new(
        StatusCodes.Status500InternalServerError,
        "Internal server error"
        );

    public static readonly ErrorCode DatabaseError = new(
        StatusCodes.Status500InternalServerError,
        "Database error"
        );

    public static readonly ErrorCode Timeout = new(
        StatusCodes.Status504GatewayTimeout,
        "Request timeout"
        );

    public static readonly ErrorCode ExternalServiceError = new(
        StatusCodes.Status502BadGateway,
        "External service error"
        );

    public static readonly ErrorCode InvalidJson = new(
        StatusCodes.Status400BadRequest,
        "Invalid JSON payload"
        );
}