using AuthService.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Configurations;

public sealed class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable(
            "sys_user_sessions",
            table =>
            {
                table.HasCheckConstraint(
                    "chk_user_sessions_expiry",
                    "expires_at > created_at");

                table.HasCheckConstraint(
                    "chk_user_sessions_risk_score",
                    "risk_score IS NULL OR (risk_score >= 0 AND risk_score <= 100)");
            });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.AccessTokenJti)
            .HasColumnName("access_token_jti")
            .IsRequired();

        builder.Property(x => x.SessionFamilyId)
            .HasColumnName("session_family_id")
            .HasDefaultValueSql("gen_random_uuid()")
            .IsRequired();

        builder.Property(x => x.RefreshTokenFamilyId)
            .HasColumnName("refresh_token_family_id")
            .HasDefaultValueSql("gen_random_uuid()")
            .IsRequired();

        builder.Property(x => x.IpAddress)
            .HasColumnName("ip_address")
            .HasColumnType("inet");

        builder.Property(x => x.UserAgent)
            .HasColumnName("user_agent")
            .HasColumnType("text");

        builder.Property(x => x.DeviceId)
            .HasColumnName("device_id")
            .HasMaxLength(255);

        builder.Property(x => x.DeviceName)
            .HasColumnName("device_name")
            .HasMaxLength(255);

        builder.Property(x => x.DeviceFingerprintHash)
            .HasColumnName("device_fingerprint_hash")
            .HasMaxLength(255);

        builder.Property(x => x.MfaVerified)
            .HasColumnName("mfa_verified")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(x => x.RiskScore)
            .HasColumnName("risk_score")
            .HasPrecision(5, 2);

        builder.Property(x => x.AuthTime)
            .HasColumnName("auth_time")
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        builder.Property(x => x.ExpiresAt)
            .HasColumnName("expires_at")
            .IsRequired();

        builder.Property(x => x.AbsoluteExpiresAt)
            .HasColumnName("absolute_expires_at");

        builder.Property(x => x.IdleExpiresAt)
            .HasColumnName("idle_expires_at");

        builder.Property(x => x.LastAccessedAt)
            .HasColumnName("last_accessed_at")
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        builder.Property(x => x.LastSeenAt)
            .HasColumnName("last_seen_at")
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(x => x.RevokedAt)
            .HasColumnName("revoked_at");

        builder.Property(x => x.RevokedBy)
            .HasColumnName("revoked_by");

        builder.Property(x => x.RevokedReason)
            .HasColumnName("revoked_reason")
            .HasColumnType("text");

        builder.Property(x => x.LogoutAt)
            .HasColumnName("logout_at");

        builder.Property(x => x.Noted)
            .HasColumnName("noted")
            .HasColumnType("text");

        builder.Property(x => x.Metadata)
            .HasColumnName("metadata")
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'{}'::jsonb")
            .IsRequired();

        builder.HasIndex(x => new
            {
                x.UserId,
                x.IsActive
            })
            .HasDatabaseName("ix_user_sessions_user");

        builder.HasIndex(x => x.RefreshTokenFamilyId)
            .HasDatabaseName("ix_user_sessions_family");

        builder.HasIndex(x => x.ExpiresAt)
            .HasDatabaseName("ix_user_sessions_expires_at");

        builder.HasOne(x => x.User)
            .WithMany(x => x.Sessions)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.RevokedByUser)
            .WithMany()
            .HasForeignKey(x => x.RevokedBy)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasMany(x => x.InvalidTokens)
            .WithOne(x => x.Session)
            .HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}