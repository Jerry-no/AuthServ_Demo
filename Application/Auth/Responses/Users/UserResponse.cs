using System.Text.Json;
using AuthService.Common.Constants;

namespace AuthService.Application.Auth.Responses.Users;

public sealed class UserResponse
{
    public Guid Id { get; init; }

    public string Username { get; init; } = null!;

    public string? Email { get; init; }

    public string? PhoneNumber { get; init; }

    public string? FullName { get; init; }

    public bool Enabled { get; init; }

    public UserStatus Status { get; init; }

    public bool MfaEnabled { get; init; }

    public bool MustChangePassword { get; init; }

    public DateTimeOffset PasswordSetAt { get; init; }  //time set pass

    public DateTimeOffset? PasswordChangedAt { get; init; }  // time change pass

    public DateTimeOffset? PasswordExpiresAt { get; init; }  // expiration pass

    public bool IsEmailVerified { get; init; }

    public bool IsPhoneVerified { get; init; }

    public DateTimeOffset? LastLoginAt { get; init; }

    public DateTimeOffset? LockedUntil { get; init; }

    public JsonDocument? Metadata { get; init; }
}