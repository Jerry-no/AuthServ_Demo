using System.Text.Json;
using AuthService.Common.Constants;

namespace AuthService.Domain.Entity;


public sealed class UserMfaMethod
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public UserMfaMethodType MethodType { get; set; }

    public string? SecretEncrypted { get; set; }

    public string? CredentialId { get; set; }

    public string? PublicKey { get; set; }

    public string? DisplayName { get; set; }

    public bool Enabled { get; set; }

    public DateTimeOffset? VerifiedAt { get; set; }

    public DateTimeOffset? LastUsedAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public JsonDocument Metadata { get; set; } = JsonDocument.Parse("{}");

    public User User { get; set; } = null!;
}
