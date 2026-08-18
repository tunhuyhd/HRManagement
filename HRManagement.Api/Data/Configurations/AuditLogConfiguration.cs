using HRManagement.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.Api.Data.Configurations;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(auditLog => auditLog.Id);

        builder.Property(auditLog => auditLog.TableName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(auditLog => auditLog.RecordId)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(auditLog => auditLog.Action)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(auditLog => auditLog.ChangedColumns)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(auditLog => auditLog.OldValues)
            .HasColumnType("jsonb");

        builder.Property(auditLog => auditLog.NewValues)
            .HasColumnType("jsonb");

        builder.Property(auditLog => auditLog.ChangedByEmail)
            .HasMaxLength(256);

        builder.HasIndex(auditLog => new
        {
            auditLog.TableName,
            auditLog.RecordId
        });

        builder.HasIndex(auditLog => auditLog.ChangedAtUtc);
    }
}
