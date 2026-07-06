using QAuditLogService.Domain.Enums;

namespace QAuditLogService.Domain.Models;

public class AuditLog
{
    public int Id { get; set; }

    public int? UserId { get; set; }
    public string? UserName { get; set; }

    public AuditAction Action { get; set; }

    public string ServiceName { get; set; }

    public string EntityName { get; set; } 

    public int? EntityId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public List<AuditLogDetails> AuditLogDetails { get; set; } = [];

    public bool IsSuccess { get; set; } = true;
    public string? ErrorMessage { get; set; }
}