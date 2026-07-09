using AuthService.Common.Constants;
using AuthService.Common.Helpers;
using AuthService.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Configurations;

public sealed class UserMfaMethodConfiguration
    : IEntityTypeConfiguration<UserMfaMethod>
{
    public void Configure(EntityTypeBuilder<UserMfaMethod> builder)
    {
        builder.ToTable(
            "sys_user_mfa_methods",
            table =>
            {
                table.HasCheckConstraint(
                    "chk_user_mfa_methods_type",
                    "method_type IN ('TOTP', 'EMAIL', 'SMS', 'WEBAUTHN')");

                table.HasCheckConstraint(
                    "chk_user_mfa_methods_verified",
                    """
                    enabled = FALSE
                    OR verified_at IS NOT NULL
                    OR method_type IN ('EMAIL', 'SMS')
                    """);
            });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.MethodType)
            .HasConversion<UpperCaseEnumConverter<UserMfaMethodType>>()
            .HasColumnName("method_type")
            .HasMaxLength(20);
            
        builder.Property(x => x.SecretEncrypted)
            .HasColumnName("secret_encrypted")
            .HasColumnType("text");

        builder.Property(x => x.CredentialId)
            .HasColumnName("credential_id")
            .HasColumnType("text");

        builder.Property(x => x.PublicKey)
            .HasColumnName("public_key")
            .HasColumnType("text");

        builder.Property(x => x.DisplayName)
            .HasColumnName("display_name")
            .HasMaxLength(150);

        builder.Property(x => x.Enabled)
            .HasColumnName("enabled")
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(x => x.VerifiedAt)
            .HasColumnName("verified_at");

        builder.Property(x => x.LastUsedAt)
            .HasColumnName("last_used_at");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        builder.Property(x => x.Metadata)
            .HasColumnName("metadata")
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'{}'::jsonb")
            .IsRequired();

        builder.HasIndex(x => new
            {
                x.UserId,
                x.Enabled
            })
            .HasDatabaseName("ix_user_mfa_methods_user");

        builder.HasIndex(x => x.CredentialId)
            .HasDatabaseName("ux_user_mfa_webauthn_credential")
            .IsUnique()
            .HasFilter("credential_id IS NOT NULL");
        
        builder.HasOne(x => x.User)
            .WithMany(x => x.MfaMethods)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}