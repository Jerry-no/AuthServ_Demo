using System.Text.Json;
using AuthService.Common.Constants;
using AuthService.Common.Models;

namespace AuthService.Domain.Entity;

public sealed class Permission : EntityBase
{
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string ResourceType { get; set; } = null!;

    public string Action { get; set; } = null!;

    public string? Description { get; set; }

    public PermissionStatus  Status { get; set; }

    public Guid? CreatedBy { get; set; }
    
    public Guid? UpdatedBy { get; set; }

    public JsonDocument Metadata { get; set; } = JsonDocument.Parse("{}");
    public ICollection<RolePermission> RolePermissions { get; set; } = [];
}