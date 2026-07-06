namespace QAuditLogService.Domain.Models;

public class AuditLogDetails
{
    public string PropertyName { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }

}