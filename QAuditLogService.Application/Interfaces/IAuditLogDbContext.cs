using Microsoft.EntityFrameworkCore;
using QAuditLogService.Domain.Models;

namespace QAuditLogService.Application.Interfaces;

public interface IAuditLogDbContext
{
    DbSet<AuditLog> AuditLogs { get; set; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    

}