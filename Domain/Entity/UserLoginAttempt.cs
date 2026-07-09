using System.Net;
using System.Text.Json;

namespace AuthService.Domain.Entity;

public sealed class UserLoginAttempt
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public string? UsernameOrEmail { get; set; }

    public bool Success { get; set; }

    public string? FailureReason { get; set; }

    public IPAddress? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public string? DeviceId { get; set; }

    public string? DeviceFingerprintHash { get; set; }

    public decimal? RiskScore { get; set; }

    public string? TraceId { get; set; }

    public string? CorrelationId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public JsonDocument Metadata { get; set; } = JsonDocument.Parse("{}");

    public User? User { get; set; }
}