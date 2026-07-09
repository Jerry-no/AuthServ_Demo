using AuthService.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Configurations;

public sealed class AuthorizationDecisionLogConfiguration
    : IEntityTypeConfiguration<AuthorizationDecisionLog>
{
    public void Configure(EntityTypeBuilder<AuthorizationDecisionLog> builder)
    {
        builder.ToTable("sys_authorization_decision_logs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.UserId)
            .HasColumnName("user_id");

        builder.Property(x => x.SessionId)
            .HasColumnName("session_id");

        builder.Property(x => x.ResourceType)
            .HasColumnName("resource_type")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.ResourceId)
            .HasColumnName("resource_id");

        builder.Property(x => x.Action)
            .HasColumnName("action")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Allowed)
            .HasColumnName("allowed")
            .IsRequired();

        builder.Property(x => x.MatchedPolicyId)
            .HasColumnName("matched_policy_id");

        builder.Property(x => x.DenyReason)
            .HasColumnName("deny_reason")
            .HasColumnType("text");

        builder.Property(x => x.SubjectAttributes)
            .HasColumnName("subject_attributes")
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'{}'::jsonb")
            .IsRequired();

        builder.Property(x => x.ResourceAttributes)
            .HasColumnName("resource_attributes")
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'{}'::jsonb")
            .IsRequired();

        builder.Property(x => x.EnvironmentAttributes)
            .HasColumnName("environment_attributes")
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'{}'::jsonb")
            .IsRequired();

        builder.Property(x => x.IpAddress)
            .HasColumnName("ip_address")
            .HasColumnType("inet");

        builder.Property(x => x.UserAgent)
            .HasColumnName("user_agent")
            .HasColumnType("text");

        builder.Property(x => x.TraceId)
            .HasColumnName("trace_id")
            .HasMaxLength(100);

        builder.Property(x => x.CorrelationId)
            .HasColumnName("correlation_id")
            .HasMaxLength(100);

        builder.Property(x => x.EvaluationTimeMs)
            .HasColumnName("evaluation_time_ms");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        builder.HasIndex(x => new
            {
                x.UserId,
                x.CreatedAt
            })
            .HasDatabaseName("ix_auth_decision_logs_user_created")
            .IsDescending(false, true);

        builder.HasIndex(x => new
            {
                x.ResourceType,
                x.ResourceId,
                x.Action
            })
            .HasDatabaseName("ix_auth_decision_logs_resource");

        builder.HasIndex(x => new
            {
                x.Allowed,
                x.CreatedAt
            })
            .HasDatabaseName("ix_auth_decision_logs_allowed")
            .IsDescending(false, true);

        builder.HasIndex(x => x.TraceId)
            .HasDatabaseName("ix_auth_decision_logs_trace");

        builder.HasOne(x => x.User)
            .WithMany(x => x.AuthorizationDecisionLogs)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Session)
            .WithMany(x => x.AuthorizationDecisionLogs)
            .HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.MatchedPolicy)
            .WithMany(x => x.AuthorizationDecisionLogs)
            .HasForeignKey(x => x.MatchedPolicyId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}