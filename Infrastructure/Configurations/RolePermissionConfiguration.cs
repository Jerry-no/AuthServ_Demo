using AuthService.Common.Constants;
using AuthService.Common.Helpers;
using AuthService.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Configurations;

public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("sys_role_permissions");

        builder.HasKey(x => new
        {
            x.RoleId,
            x.PermissionId
        });

        builder.Property(x => x.RoleId)
            .HasColumnName("role_id");

        builder.Property(x => x.PermissionId)
            .HasColumnName("permission_id");

        builder.Property(x => x.Status)
            .HasConversion<UpperCaseEnumConverter<RolePermissionStatus>>()
            .HasColumnName("status")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.AssignedAt)
            .HasColumnName("assigned_at")
            .IsRequired();

        builder.Property(x => x.AssignedBy)
            .HasColumnName("assigned_by");

        builder.Property(x => x.Noted)
            .HasColumnName("noted");

        builder.HasIndex(x => new
            {
                x.PermissionId,
                x.Status
            })
            .HasDatabaseName("ix_sys_role_permissions_permission");
        
        builder.HasKey(x => new
        {
            x.RoleId,
            x.PermissionId
        });
        
        builder.HasOne(x => x.Role)
            .WithMany(x => x.RolePermissions)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Permission)
            .WithMany(x => x.RolePermissions)
            .HasForeignKey(x => x.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.AssignedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}