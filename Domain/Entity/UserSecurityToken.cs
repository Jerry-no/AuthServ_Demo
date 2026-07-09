using System.Net;
using System.Text.Json;
using AuthService.Common.Constants;

namespace AuthService.Domain.Entity;

public sealed class UserSecurityToken
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public SecurityTokenPurpose Purpose { get; set; }

    public string TokenHash { get; set; } = null!;

    public string? TargetValue { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }

    public DateTimeOffset? ConsumedAt { get; set; }

    public IPAddress? CreatedByIp { get; set; }

    public IPAddress? ConsumedByIp { get; set; }

    public string? UserAgent { get; set; }

    public JsonDocument Metadata { get; set; }
        = JsonDocument.Parse("{}");

    public User? User { get; set; }
}