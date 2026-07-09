using System.Net;
using System.Text.Json;

namespace AuthService.Domain.Entity;

public sealed class TrustedDevice
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string DeviceId { get; set; } = null!;

    public string? DeviceName { get; set; }

    public string DeviceFingerprintHash { get; set; } = null!;

    public DateTimeOffset TrustedAt { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }

    public DateTimeOffset? LastUsedAt { get; set; }

    public DateTimeOffset? RevokedAt { get; set; }

    public string? RevokedReason { get; set; }

    public IPAddress? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public JsonDocument Metadata { get; set; } = JsonDocument.Parse("{}");

    public User User { get; set; } = null!;
}