using AuthService.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Configurations;

public sealed class TrustedDeviceConfiguration : IEntityTypeConfiguration<TrustedDevice>
{
    public void Configure(EntityTypeBuilder<TrustedDevice> builder)
    {
        builder.ToTable(
            "sys_trusted_devices",
            table =>
            {
                table.HasCheckConstraint(
                    "chk_trusted_devices_expiry",
                    "expires_at > trusted_at");
            });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.DeviceId)
            .HasColumnName("device_id")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.DeviceName)
            .HasColumnName("device_name")
            .HasMaxLength(255);

        builder.Property(x => x.DeviceFingerprintHash)
            .HasColumnName("device_fingerprint_hash")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.TrustedAt)
            .HasColumnName("trusted_at")
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        builder.Property(x => x.ExpiresAt)
            .HasColumnName("expires_at")
            .IsRequired();

        builder.Property(x => x.LastUsedAt)
            .HasColumnName("last_used_at");

        builder.Property(x => x.RevokedAt)
            .HasColumnName("revoked_at");

        builder.Property(x => x.RevokedReason)
            .HasColumnName("revoked_reason")
            .HasColumnType("text");

        builder.Property(x => x.IpAddress)
            .HasColumnName("ip_address")
            .HasColumnType("inet");

        builder.Property(x => x.UserAgent)
            .HasColumnName("user_agent")
            .HasColumnType("text");

        builder.Property(x => x.Metadata)
            .HasColumnName("metadata")
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'{}'::jsonb")
            .IsRequired();

        builder.HasIndex(x => new
            {
                x.UserId,
                x.DeviceId
            })
            .HasDatabaseName("ux_trusted_devices_user_device_active")
            .IsUnique()
            .HasFilter("revoked_at IS NULL");

        builder.HasIndex(x => new
            {
                x.UserId,
                x.RevokedAt,
                x.ExpiresAt
            })
            .HasDatabaseName("ix_trusted_devices_user");

        builder.HasOne(x => x.User)
            .WithMany(x => x.TrustedDevices)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}