using AuthService.Common.Abstractions;
using AuthService.Common.Constants;
using AuthService.Common.Helpers;
using AuthService.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Configurations;
public sealed class UserConfiguration
    : EntityBaseConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.ToTable("sys_users");

        builder.Property(x => x.Username)
            .HasColumnName("username")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasColumnName("email")
            .HasMaxLength(255);

        builder.Property(x => x.PhoneNumber)
            .HasColumnName("phone_number")
            .HasMaxLength(50);

        builder.Property(x => x.PasswordHash)
            .HasColumnName("password_hash")
            .IsRequired();

        builder.Property(x => x.PasswordAlgorithm)
            .HasColumnName("password_algorithm")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.PasswordHashVersion)
            .HasColumnName("password_hash_version");

        builder.Property(x => x.PasswordSetAt)
            .HasColumnName("password_set_at");

        builder.Property(x => x.PasswordChangedAt)
            .HasColumnName("password_changed_at");

        builder.Property(x => x.PasswordExpiresAt)
            .HasColumnName("password_expires_at");

        builder.Property(x => x.MustChangePassword)
            .HasColumnName("must_change_password");

        builder.Property(x => x.FullName)
            .HasColumnName("full_name")
            .HasMaxLength(255);

        builder.Property(x => x.EmailVerifiedAt)
            .HasColumnName("email_verified_at");

        builder.Property(x => x.PhoneVerifiedAt)
            .HasColumnName("phone_verified_at");

        builder.Property(x => x.Enabled)
            .HasColumnName("enabled");

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<UpperCaseEnumConverter<UserStatus>>()
            .HasMaxLength(50);

        builder.Property(x => x.LastLoginAt)
            .HasColumnName("last_login_at");

        builder.Property(x => x.LastFailedLoginAt)
            .HasColumnName("last_failed_login_at");

        builder.Property(x => x.FailedLoginCount)
            .HasColumnName("failed_login_count");

        builder.Property(x => x.LockedUntil)
            .HasColumnName("locked_until");

        builder.Property(x => x.MfaEnabled)
            .HasColumnName("mfa_enabled");

        builder.Property(x => x.MfaVerifiedAt)
            .HasColumnName("mfa_verified_at");

        builder.Property(x => x.SecurityStamp)
            .HasColumnName("security_stamp")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.TokenVersion)
            .HasColumnName("token_version");

        builder.Property(x => x.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(x => x.UpdatedBy)
            .HasColumnName("updated_by");

        builder.Property(x => x.Metadata)
            .HasColumnName("metadata")
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'{}'::jsonb")
            .IsRequired();
        builder.HasIndex(x => x.Username)
            .HasDatabaseName("ux_sys_users_username_active")
            .IsUnique()
            .HasFilter("is_deleted = FALSE");

        builder.HasIndex(x => x.Email)
            .HasDatabaseName("ux_sys_users_email_active")
            .IsUnique()
            .HasFilter("email IS NOT NULL AND is_deleted = FALSE");

        builder.HasIndex(x => x.PhoneNumber)
            .HasDatabaseName("ux_sys_users_phone_active")
            .IsUnique()
            .HasFilter("phone_number IS NOT NULL AND is_deleted = FALSE");

        builder.HasIndex(x => new
            {
                x.Status,
                x.Enabled,
                x.IsDeleted
            })
            .HasDatabaseName("ix_sys_users_status");;
    }
}