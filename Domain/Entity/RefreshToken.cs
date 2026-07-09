using System.Net;
using System.Text.Json;
using AuthService.Common.Constants;

namespace AuthService.Domain.Entity;
public sealed class RefreshToken
{
    public Guid Id { get; set; }

    public Guid SessionId { get; set; }

    public Guid UserId { get; set; }

    public Guid FamilyId { get; set; }

    public string TokenHash { get; set; } = null!;

    public Guid Jti { get; set; }

    public Guid? ParentTokenId { get; set; }

    public Guid? ReplacedByTokenId { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public IPAddress? CreatedByIp { get; set; }

    public DateTimeOffset? UsedAt { get; set; }

    public DateTimeOffset? RotatedAt { get; set; }

    public DateTimeOffset? RevokedAt { get; set; }

    public IPAddress? RevokedByIp { get; set; }

    public string? RevokedReason { get; set; }

    public DateTimeOffset? ReuseDetectedAt { get; set; }

    public RefreshTokenStatus Status { get; set; }

    public JsonDocument Metadata { get; set; }
        = JsonDocument.Parse("{}");

    public User User { get; set; } = null!;

    public UserSession Session { get; set; } = null!;

    public RefreshToken? ParentToken { get; set; }
    
    public ICollection<RefreshToken> ChildTokens { get; set; } = [];

}