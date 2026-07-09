using AuthService.Common.Abstractions;
using AuthService.Common.Constants;
using AuthService.Common.Helpers;
using AuthService.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Configurations;

public sealed class PermissionConfiguration : EntityBaseConfiguration<Permission>
{
    public override void Configure(EntityTypeBuilder<Permission> builder)
    {
        base.Configure(builder);

        builder.ToTable("sys_permissions");

        builder.Property(x => x.Code)
            .HasColumnName("code")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.ResourceType)
            .HasColumnName("resource_type")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Action)
            .HasColumnName("action")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(1000);

        builder.Property(x => x.Status)
            .HasConversion<UpperCaseEnumConverter<PermissionStatus>>()
            .HasColumnName("status")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(x => x.UpdatedBy)
            .HasColumnName("updated_by");

        builder.Property(x => x.Metadata)
            .HasColumnName("metadata")
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'{}'::jsonb")
            .IsRequired();

        builder.HasIndex(x => x.Code)
            .HasDatabaseName("ux_sys_permissions_code_active")
            .IsUnique()
            .HasFilter("is_deleted = FALSE");

        builder.HasIndex(x => new
            {
                x.ResourceType,
                x.Action
            })
            .HasDatabaseName("ux_sys_permissions_resource_action_active")
            .IsUnique()
            .HasFilter("is_deleted = FALSE");

        builder.HasIndex(x => new
            {
                x.ResourceType,
                x.Action,
                x.Status
            })
            .HasDatabaseName("ix_sys_permissions_lookup");

        builder.HasMany(x => x.RolePermissions)
            .WithOne(x => x.Permission)
            .HasForeignKey(x => x.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}