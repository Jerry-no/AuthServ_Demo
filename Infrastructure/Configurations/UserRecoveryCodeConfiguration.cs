using AuthService.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Configurations;

public sealed class UserRecoveryCodeConfiguration
    : IEntityTypeConfiguration<UserRecoveryCode>
{
    public void Configure(EntityTypeBuilder<UserRecoveryCode> builder)
    {
        builder.ToTable(
            "sys_user_recovery_codes",
            table =>
            {
                table.HasCheckConstraint(
                    "chk_recovery_codes_expiry",
                    "expires_at IS NULL OR expires_at > created_at");
            });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.CodeHash)
            .HasColumnName("code_hash")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.UsedAt)
            .HasColumnName("used_at");

        builder.Property(x => x.UsedByIp)
            .HasColumnName("used_by_ip")
            .HasColumnType("inet");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        builder.Property(x => x.ExpiresAt)
            .HasColumnName("expires_at");

        builder.HasIndex(x => new
            {
                x.UserId,
                x.UsedAt
            })
            .HasDatabaseName("ix_user_recovery_codes_user");

        builder.HasOne(x => x.User)
            .WithMany(x => x.RecoveryCodes)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}