namespace AuthService.Common.Security;

public sealed class SecurityOptions
{
    public int MaxFailedLoginAttempts { get; init; } = 5;

    public int LockoutMinutes { get; init; } = 15;

    public int PasswordMinLength { get; init; } = 8;

    public bool RequireUppercase { get; init; } = true;

    public bool RequireLowercase { get; init; } = true;

    public bool RequireDigit { get; init; } = true;

    public bool RequireSpecialCharacter { get; init; } = true;

    public int RefreshTokenExpirationDays { get; init; } = 30;

    public int SessionExpirationMinutes { get; init; } = 60;

    public int AbsoluteSessionExpirationDays { get; init; } = 30;

    public int IdleSessionExpirationMinutes { get; init; } = 30;

    public int EmailConfirmationTokenMinutes { get; init; } = 30;

    public int PasswordResetTokenMinutes { get; init; } = 15;
}