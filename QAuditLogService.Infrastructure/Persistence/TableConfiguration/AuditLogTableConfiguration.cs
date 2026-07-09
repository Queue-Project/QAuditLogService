using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QAuditLogService.Domain.Models;

namespace QAuditLogService.Infrastructure.Persistence.TableConfiguration;

public class AuditLogTableConfiguration: IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(s => s.Id);
        builder.HasIndex(s => s.UserId);
        builder.HasIndex(s => s.EntityId);
        builder.Property(s => s.EntityName)
            .IsRequired();

        builder.Property(s => s.AuditLogDetails)
            .HasColumnType("jsonb");

        builder.Property(s => s.Action)
            .IsRequired();
    }
}