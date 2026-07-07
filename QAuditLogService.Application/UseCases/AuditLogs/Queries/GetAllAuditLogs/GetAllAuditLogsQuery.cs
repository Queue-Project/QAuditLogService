using MediatR;
using QAuditLogService.Application.Responses;

namespace QAuditLogService.Application.UseCases.AuditLogs.Queries.GetAllAuditLogs;

public record GetAllAuditLogsQuery(
    int? UserId,
    string Action,
    string EntityName,
    string ServiceName,
    DateTime? From,
    DateTime? To,
    int PageNumber=1,
    int PageSize=10) : IRequest<PagedResponse<AuditResponse>>;