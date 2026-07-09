
namespace AuthService.Application.Auth.Commands.Login;

public sealed class AuthResponse
{
    public Guid UserId { get; init; }

    public string Username { get; init; } = null!;

    public string? FullName { get; init; }

    public string AccessToken { get; init; } = null!;

    public string RefreshToken { get; init; } = null!;

    public DateTimeOffset ExpiresAt { get; init; }

    public bool MustChangePassword { get; init; }

    public bool MfaRequired { get; init; }
}