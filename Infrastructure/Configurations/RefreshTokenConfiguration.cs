using AuthService.Common.Constants;
using AuthService.Common.Helpers;
using AuthService.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Configurations;

public sealed class RefreshTokenConfiguration
    : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable(
            "sys_refresh_tokens",
            table =>
            {
                table.HasCheckConstraint(
                    "chk_refresh_tokens_expiry",
                    "expires_at > created_at");
            });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.SessionId)
            .HasColumnName("session_id")
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.FamilyId)
            .HasColumnName("family_id")
            .IsRequired();

        builder.Property(x => x.TokenHash)
            .HasColumnName("token_hash")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.Jti)
            .HasColumnName("jti")
            .IsRequired();

        builder.Property(x => x.ParentTokenId)
            .HasColumnName("parent_token_id");

        builder.Property(x => x.ReplacedByTokenId)
            .HasColumnName("replaced_by_token_id");

        builder.Property(x => x.ExpiresAt)
            .HasColumnName("expires_at")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        builder.Property(x => x.CreatedByIp)
            .HasColumnName("created_by_ip")
            .HasColumnType("inet");

        builder.Property(x => x.UsedAt)
            .HasColumnName("used_at");

        builder.Property(x => x.RotatedAt)
            .HasColumnName("rotated_at");

        builder.Property(x => x.RevokedAt)
            .HasColumnName("revoked_at");

        builder.Property(x => x.RevokedByIp)
            .HasColumnName("revoked_by_ip")
            .HasColumnType("inet");

        builder.Property(x => x.RevokedReason)
            .HasColumnName("revoked_reason")
            .HasColumnType("text");

        builder.Property(x => x.ReuseDetectedAt)
            .HasColumnName("reuse_detected_at");

        builder.Property(x => x.Status)
            .HasConversion<UpperCaseEnumConverter<RefreshTokenStatus>>()
            .HasColumnName("status")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Metadata)
            .HasColumnName("metadata")
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'{}'::jsonb")
            .IsRequired();

        builder.HasIndex(x => x.SessionId)
            .HasDatabaseName("ix_refresh_tokens_session");

        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("ix_refresh_tokens_user");

        builder.HasIndex(x => x.FamilyId)
            .HasDatabaseName("ix_refresh_tokens_family");

        builder.HasIndex(x => x.ExpiresAt)
            .HasDatabaseName("ix_refresh_tokens_expires_at");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("ix_refresh_tokens_status");

        builder.HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Session)
            .WithMany()
            .HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ParentToken)
            .WithMany(x => x.ChildTokens)
            .HasForeignKey(x => x.ParentTokenId)
            .OnDelete(DeleteBehavior.SetNull);
        
    }
}