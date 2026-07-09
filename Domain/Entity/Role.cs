using System.Text.Json;
using AuthService.Common.Constants;
using AuthService.Common.Models;

namespace AuthService.Domain.Entity;

public sealed class Role : EntityBase
{
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsSystem { get; set; }

    public RoleStatus Status { get; set; }
    
    public Guid? CreatedBy { get; set; }
    
    public Guid? UpdatedBy { get; set; }
    
    public JsonDocument Metadata { get; set; } = JsonDocument.Parse("{}");
    
    public ICollection<UserRole> UserRoles { get; set; } = [];

    public ICollection<RolePermission> RolePermissions { get; set; } = [];
}