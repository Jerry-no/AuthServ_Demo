using System.Text.Json;
using AuthService.Common.Constants;
using AuthService.Common.Models;

namespace AuthService.Domain.Entity;

public sealed class AbacPolicy : EntityBase
{
    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string ResourceType { get; set; } = null!;

    public string Action { get; set; } = null!;

    public AbacPolicyEffect Effect { get; set; }

    public int Priority { get; set; }

    public int Version { get; set; }

    public JsonDocument ConditionExpression { get; set; } = JsonDocument.Parse("{}");

    public AbacPolicyStatus Status { get; set; }

    public Guid? CreatedBy { get; set; }

    public Guid? UpdatedBy { get; set; }

    public JsonDocument Metadata { get; set; } = JsonDocument.Parse("{}");

    public User? CreatedByUser { get; set; }

    public User? UpdatedByUser { get; set; }
    
    public ICollection<AuthorizationDecisionLog> AuthorizationDecisionLogs { get; set; } = [];
}