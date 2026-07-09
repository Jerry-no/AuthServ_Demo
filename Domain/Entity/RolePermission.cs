using AuthService.Common.Constants;

namespace AuthService.Domain.Entity;

public sealed class RolePermission
{
    public Guid RoleId { get; set; }

    public Guid PermissionId { get; set; }

    public RolePermissionStatus Status { get; set; }


    public DateTimeOffset AssignedAt { get; set; }

    public Guid? AssignedBy { get; set; }

    public string? Noted { get; set; }

    public Role Role { get; set; } = null!;

    public Permission Permission { get; set; } = null!;
}