using System.Net;
using System.Text.Json;

namespace AuthService.Domain.Entity;

public sealed class AuthorizationDecisionLog
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public Guid? SessionId { get; set; }

    public string ResourceType { get; set; } = null!;

    public Guid? ResourceId { get; set; }

    public string Action { get; set; } = null!;

    public bool Allowed { get; set; }

    public Guid? MatchedPolicyId { get; set; }

    public string? DenyReason { get; set; }

    public JsonDocument SubjectAttributes { get; set; }
        = JsonDocument.Parse("{}");

    public JsonDocument ResourceAttributes { get; set; }
        = JsonDocument.Parse("{}");

    public JsonDocument EnvironmentAttributes { get; set; }
        = JsonDocument.Parse("{}");

    public IPAddress? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public string? TraceId { get; set; }

    public string? CorrelationId { get; set; }

    public int? EvaluationTimeMs { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public User? User { get; set; }

    public UserSession? Session { get; set; }

    public AbacPolicy? MatchedPolicy { get; set; }
}