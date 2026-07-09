namespace AuthService.Common.ErrorCodes;

public sealed record ErrorCode(
    int Code,
    string Message
);