using QAuditLogService.Contracts;

namespace QAuditLogService.Application.Responses;

public class AuditResponse
{
    public DateTimeOffset OccurredAt { get; set; }
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; }
    public int EntityId { get; set; }
    public string EntityName { get; set; }
    public string Action { get; set; }
    public string ServiceName { get; set; }
    public List<AuditEventLogDetails> LogDetailsList { get; set; } = [];
}