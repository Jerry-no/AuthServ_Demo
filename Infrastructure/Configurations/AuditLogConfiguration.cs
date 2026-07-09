using AuthService.Common.Constants;
using AuthService.Common.Helpers;
using AuthService.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Configurations;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("sys_audit_logs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.UserId)
            .HasColumnName("user_id");

        builder.Property(x => x.SessionId)
            .HasColumnName("session_id");

        builder.Property(x => x.ActorType)
            .HasConversion<UpperCaseEnumConverter<AuditActorType>>()
            .HasMaxLength(20)
            .HasDefaultValue(AuditActorType.User)
            .IsRequired();

        builder.Property(x => x.Action)
            .HasColumnName("action")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.ResourceType)
            .HasColumnName("resource_type")
            .HasMaxLength(100);

        builder.Property(x => x.ResourceId)
            .HasColumnName("resource_id");

        builder.Property(x => x.HttpMethod)
            .HasColumnName("http_method")
            .HasMaxLength(20);

        builder.Property(x => x.RequestPath)
            .HasColumnName("request_path")
            .HasColumnType("text");

        builder.Property(x => x.IpAddress)
            .HasColumnName("ip_address")
            .HasColumnType("inet");

        builder.Property(x => x.UserAgent)
            .HasColumnName("user_agent")
            .HasColumnType("text");

        builder.Property(x => x.Success)
            .HasColumnName("success")
            .IsRequired();

        builder.Property(x => x.ErrorMessage)
            .HasColumnName("error_message")
            .HasColumnType("text");

        builder.Property(x => x.BeforeData)
            .HasColumnName("before_data")
            .HasColumnType("jsonb");

        builder.Property(x => x.AfterData)
            .HasColumnName("after_data")
            .HasColumnType("jsonb");

        builder.Property(x => x.TraceId)
            .HasColumnName("trace_id")
            .HasMaxLength(100);

        builder.Property(x => x.CorrelationId)
            .HasColumnName("correlation_id")
            .HasMaxLength(100);

        builder.Property(x => x.Metadata)
            .HasColumnName("metadata")
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'{}'::jsonb")
            .IsRequired();

        builder.Property(x => x.Hash)
            .HasColumnName("hash")
            .HasMaxLength(128);

        builder.Property(x => x.PreviousHash)
            .HasColumnName("previous_hash")
            .HasMaxLength(128);

        builder.Property(x => x.Status)
            .HasConversion<UpperCaseEnumConverter<AuditLogStatus>>()
            .HasColumnName("status")
            .HasMaxLength(20)
            .HasDefaultValue(AuditLogStatus.Active)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(x => x.Noted)
            .HasColumnName("noted")
            .HasColumnType("text");

        builder.HasIndex(x => new
            {
                x.UserId,
                x.CreatedAt
            })
            .HasDatabaseName("ix_audit_logs_user_created")
            .IsDescending(false, true);

        builder.HasIndex(x => new
            {
                x.ResourceType,
                x.ResourceId
            })
            .HasDatabaseName("ix_audit_logs_resource");

        builder.HasIndex(x => x.TraceId)
            .HasDatabaseName("ix_audit_logs_trace");

        builder.HasIndex(x => new
            {
                x.Action,
                x.CreatedAt
            })
            .HasDatabaseName("ix_audit_logs_action_created")
            .IsDescending(false, true);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Session)
            .WithMany()
            .HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.SetNull);
    }
}