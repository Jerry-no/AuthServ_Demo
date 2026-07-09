using AuthService.Common.Constants;
using AuthService.Common.Helpers;
using AuthService.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Configurations;

public sealed class UserSecurityTokenConfiguration
    : IEntityTypeConfiguration<UserSecurityToken>
{
    public void Configure(EntityTypeBuilder<UserSecurityToken> builder)
    {
        builder.ToTable(
            "sys_user_security_tokens",
            table =>
            {
                table.HasCheckConstraint(
                    "chk_security_tokens_expiry",
                    "expires_at > created_at");
            });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.UserId)
            .HasColumnName("user_id");

        builder.Property(x => x.Purpose)
            .HasConversion<UpperCaseEnumConverter<SecurityTokenPurpose>>()
            .HasColumnName("purpose")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.TokenHash)
            .HasColumnName("token_hash")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.TargetValue)
            .HasColumnName("target_value")
            .HasMaxLength(255);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        builder.Property(x => x.ExpiresAt)
            .HasColumnName("expires_at")
            .IsRequired();

        builder.Property(x => x.ConsumedAt)
            .HasColumnName("consumed_at");

        builder.Property(x => x.CreatedByIp)
            .HasColumnName("created_by_ip")
            .HasColumnType("inet");

        builder.Property(x => x.ConsumedByIp)
            .HasColumnName("consumed_by_ip")
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
                x.Purpose,
                x.ConsumedAt
            })
            .HasDatabaseName("ix_security_tokens_user");

        builder.HasIndex(x => x.ExpiresAt)
            .HasDatabaseName("ix_security_tokens_expires_at");
        
        builder.HasOne(x => x.User)
            .WithMany(x => x.SecurityTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}