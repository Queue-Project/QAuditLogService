namespace QAuditLogService.Contracts;

public class AuditEventLogDetails
{
    public string PropertyName { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
}