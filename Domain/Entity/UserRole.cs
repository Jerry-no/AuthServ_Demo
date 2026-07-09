using AuthService.Common.Constants;

namespace AuthService.Domain.Entity;

public sealed class UserRole
{
    public Guid UserId { get; set; }

    public Guid RoleId { get; set; }

    public UserRoleStatus Status { get; set; }

    public DateTimeOffset AssignedAt { get; set; }

    public Guid? AssignedBy { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public string? Noted { get; set; }

    public User User { get; set; } = null!;

    public Role Role { get; set; } = null!;
}