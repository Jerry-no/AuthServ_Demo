using AuthService.Common.Abstractions;
using AuthService.Common.Constants;
using AuthService.Common.Helpers;
using AuthService.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Configurations;

public sealed class AbacPolicyConfiguration : EntityBaseConfiguration<AbacPolicy>
{
    public override void Configure(EntityTypeBuilder<AbacPolicy> builder)
    {
        base.Configure(builder);

        builder.ToTable("sys_abac_policies");

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Code)
            .HasColumnName("code")
            .HasColumnType("citext")
            .IsRequired();

        builder.Property(x => x.ResourceType)
            .HasColumnName("resource_type")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Action)
            .HasColumnName("action")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Effect)
            .HasConversion<UpperCaseEnumConverter<AbacPolicyEffect>>()
            .HasColumnName("effect")
            .HasMaxLength(20)
            .HasDefaultValue(AbacPolicyEffect.Allow)
            .IsRequired();

        builder.Property(x => x.Priority)
            .HasColumnName("priority")
            .HasDefaultValue(100)
            .IsRequired();

        builder.Property(x => x.Version)
            .HasColumnName("version")
            .HasDefaultValue(1)
            .IsRequired();

        builder.Property(x => x.ConditionExpression)
            .HasColumnName("condition_expression")
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'{}'::jsonb")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<UpperCaseEnumConverter<AbacPolicyStatus>>()
            .HasColumnName("status")
            .HasMaxLength(20)
            .HasDefaultValue(AbacPolicyStatus.Active)
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

        builder.HasIndex(x => new
            {
                x.Code,
                x.Version
            })
            .HasDatabaseName("ux_abac_policies_code_version_active")
            .IsUnique()
            .HasFilter("is_deleted = FALSE");

        builder.HasIndex(x => new
            {
                x.ResourceType,
                x.Action,
                x.Status,
                x.Priority
            })
            .HasDatabaseName("ix_abac_policies_lookup")
            .HasFilter("is_deleted = FALSE");

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedBy)
            .OnDelete(DeleteBehavior.SetNull);
    }
}