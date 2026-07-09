using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QAuditLogService.Contracts;
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
            .HasColumnType("jsonb")
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<AuditEventLogDetails>>(v,
                    (System.Text.Json.JsonSerializerOptions)null)
            );

        builder.Property(s => s.Action)
            .IsRequired();
    }
}