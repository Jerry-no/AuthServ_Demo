using System.Net;
using System.Text.Json;
using AuthService.Common.Constants;

namespace AuthService.Domain.Entity;

public sealed class AuditLog
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public Guid? SessionId { get; set; }

    public AuditActorType ActorType { get; set; }

    public string Action { get; set; } = null!;

    public string? ResourceType { get; set; }

    public Guid? ResourceId { get; set; }

    public string? HttpMethod { get; set; }

    public string? RequestPath { get; set; }

    public IPAddress? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public bool Success { get; set; }

    public string? ErrorMessage { get; set; }

    public JsonDocument? BeforeData { get; set; }

    public JsonDocument? AfterData { get; set; }

    public string? TraceId { get; set; }

    public string? CorrelationId { get; set; }

    public JsonDocument Metadata { get; set; } = JsonDocument.Parse("{}");

    public string? Hash { get; set; }

    public string? PreviousHash { get; set; }

    public AuditLogStatus Status { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public string? Noted { get; set; }

    public User? User { get; set; }

    public UserSession? Session { get; set; }

    public User? CreatedByUser { get; set; }
}
