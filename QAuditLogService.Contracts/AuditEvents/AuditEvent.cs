namespace QAuditLogService.Contracts.AuditEvents;

public class AuditEvent
{
    public DateTime OccuredAt { get; set; }
    public int? UserId { get; set; }
    public int? EntityId { get; set; }
    public string ServiceName { get; set; }
    public string? UserName { get; set; }
    public string EntityName { get; set; }
    public string Action { get; set; }
    public List<AuditEventLogDetails> AuditLogDetails { get; set; } = [];
    
}