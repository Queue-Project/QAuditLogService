using Microsoft.EntityFrameworkCore;
using QAuditLogService.Application.Interfaces;
using QAuditLogService.Domain.Models;
using QAuditLogService.Infrastructure.Persistence.TableConfiguration;

namespace QAuditLogService.Infrastructure.Persistence.Database;

public class AuditLogDbContext: DbContext, IAuditLogDbContext
{
    public DbSet<AuditLog> AuditLogs { get; set; }

    public AuditLogDbContext(DbContextOptions<AuditLogDbContext> options): base(options)
    {
        
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuditLogTableConfiguration).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}