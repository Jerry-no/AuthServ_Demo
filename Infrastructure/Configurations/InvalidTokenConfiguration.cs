using AuthService.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Configurations;

public sealed class InvalidTokenConfiguration
    : IEntityTypeConfiguration<InvalidToken>
{
    public void Configure(EntityTypeBuilder<InvalidToken> builder)
    {
        builder.ToTable(
            "sys_invalid_tokens",
            table =>
            {
                table.HasCheckConstraint(
                    "chk_invalid_tokens_expiry",
                    "expires_at > created_at");
            });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.Jti)
            .HasColumnName("jti")
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasColumnName("user_id");

        builder.Property(x => x.SessionId)
            .HasColumnName("session_id");

        builder.Property(x => x.Reason)
            .HasColumnName("reason")
            .HasColumnType("text");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        builder.Property(x => x.ExpiresAt)
            .HasColumnName("expires_at")
            .IsRequired();

        builder.HasIndex(x => x.ExpiresAt)
            .HasDatabaseName("ix_invalid_tokens_expires_at");

        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("ix_invalid_tokens_user");

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}