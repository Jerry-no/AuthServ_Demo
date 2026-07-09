using System.Net;
using System.Text.Json;

namespace AuthService.Domain.Entity;

public sealed class UserSession
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid AccessTokenJti { get; set; }

    public Guid SessionFamilyId { get; set; }

    public Guid RefreshTokenFamilyId { get; set; }

    public IPAddress? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public string? DeviceId { get; set; }

    public string? DeviceName { get; set; }

    public string? DeviceFingerprintHash { get; set; }

    public bool MfaVerified { get; set; }

    public decimal? RiskScore { get; set; }

    public DateTimeOffset AuthTime { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }

    public DateTimeOffset? AbsoluteExpiresAt { get; set; }

    public DateTimeOffset? IdleExpiresAt { get; set; }

    public DateTimeOffset LastAccessedAt { get; set; }

    public DateTimeOffset LastSeenAt { get; set; }

    public bool IsActive { get; set; }

    public DateTimeOffset? RevokedAt { get; set; }

    public Guid? RevokedBy { get; set; }

    public string? RevokedReason { get; set; }

    public DateTimeOffset? LogoutAt { get; set; }

    public string? Noted { get; set; }

    public JsonDocument Metadata { get; set; } = JsonDocument.Parse("{}");

    public User User { get; set; } = null!;

    public User? RevokedByUser { get; set; }
    
    public ICollection<InvalidToken> InvalidTokens { get; set; } = [];
    
    public ICollection<AuthorizationDecisionLog> AuthorizationDecisionLogs { get; set; } = [];
}