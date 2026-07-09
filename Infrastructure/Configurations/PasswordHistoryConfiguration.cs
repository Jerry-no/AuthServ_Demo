using AuthService.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Configurations;

public sealed class PasswordHistoryConfiguration
    : IEntityTypeConfiguration<PasswordHistory>
{
    public void Configure(EntityTypeBuilder<PasswordHistory> builder)
    {
        builder.ToTable("sys_password_history");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.PasswordHash)
            .HasColumnName("password_hash")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.PasswordAlgorithm)
            .HasColumnName("password_algorithm")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.PasswordHashVersion)
            .HasColumnName("password_hash_version")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        builder.Property(x => x.CreatedByIp)
            .HasColumnName("created_by_ip")
            .HasColumnType("inet");

        builder.HasIndex(x => new
            {
                x.UserId,
                x.CreatedAt
            })
            .HasDatabaseName("ix_password_history_user_created")
            .IsDescending(false, true);

        builder.HasOne(x => x.User)
            .WithMany(x => x.PasswordHistories)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}