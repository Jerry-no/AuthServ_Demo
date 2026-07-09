using AuthService.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Configurations;

public sealed class UserLoginAttemptConfiguration
    : IEntityTypeConfiguration<UserLoginAttempt>
{
    public void Configure(EntityTypeBuilder<UserLoginAttempt> builder)
    {
        builder.ToTable(
            "sys_user_login_attempts",
            table =>
            {
                table.HasCheckConstraint(
                    "chk_login_attempts_risk_score",
                    "risk_score IS NULL OR (risk_score >= 0 AND risk_score <= 100)");
            });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.UserId)
            .HasColumnName("user_id");

        builder.Property(x => x.UsernameOrEmail)
            .HasColumnName("username_or_email")
            .HasColumnType("citext");

        builder.Property(x => x.Success)
            .HasColumnName("success")
            .IsRequired();

        builder.Property(x => x.FailureReason)
            .HasColumnName("failure_reason")
            .HasMaxLength(100);

        builder.Property(x => x.IpAddress)
            .HasColumnName("ip_address")
            .HasColumnType("inet");

        builder.Property(x => x.UserAgent)
            .HasColumnName("user_agent")
            .HasColumnType("text");

        builder.Property(x => x.DeviceId)
            .HasColumnName("device_id")
            .HasMaxLength(255);

        builder.Property(x => x.DeviceFingerprintHash)
            .HasColumnName("device_fingerprint_hash")
            .HasMaxLength(255);

        builder.Property(x => x.RiskScore)
            .HasColumnName("risk_score")
            .HasPrecision(5, 2);

        builder.Property(x => x.TraceId)
            .HasColumnName("trace_id")
            .HasMaxLength(100);

        builder.Property(x => x.CorrelationId)
            .HasColumnName("correlation_id")
            .HasMaxLength(100);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
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
                x.CreatedAt
            })
            .HasDatabaseName("ix_login_attempts_user_created")
            .IsDescending(false, true);

        builder.HasIndex(x => new
            {
                x.UsernameOrEmail,
                x.CreatedAt
            })
            .HasDatabaseName("ix_login_attempts_identifier_created")
            .IsDescending(false, true);

        builder.HasIndex(x => new
            {
                x.IpAddress,
                x.CreatedAt
            })
            .HasDatabaseName("ix_login_attempts_ip_created")
            .IsDescending(false, true);

        builder.HasIndex(x => new
            {
                x.Success,
                x.CreatedAt
            })
            .HasDatabaseName("ix_login_attempts_success_created")
            .IsDescending(false, true);

        builder.HasOne(x => x.User)
            .WithMany(x => x.LoginAttempts)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}