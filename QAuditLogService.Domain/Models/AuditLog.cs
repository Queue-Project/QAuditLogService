
using QAuditLogService.Contracts;

namespace QAuditLogService.Domain.Models;

public class AuditLog
{
    public int Id { get; set; }

    public int? UserId { get; set; }
    public string? UserName { get; set; }

    public string Action { get; set; }

    public string ServiceName { get; set; }

    public string EntityName { get; set; } 

    public int? EntityId { get; set; }

    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    
    public List<AuditEventLogDetails> AuditLogDetails { get; set; } = [];
    
}