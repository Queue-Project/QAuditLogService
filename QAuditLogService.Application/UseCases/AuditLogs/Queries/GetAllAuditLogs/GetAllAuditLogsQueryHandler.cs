using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QAuditLogService.Application.Interfaces;
using QAuditLogService.Application.Responses;

namespace QAuditLogService.Application.UseCases.AuditLogs.Queries.GetAllAuditLogs;

public class GetAllAuditLogsQueryHandler: IRequestHandler<GetAllAuditLogsQuery, PagedResponse<AuditResponse>>
{
    private readonly ILogger<GetAllAuditLogsQueryHandler> _logger;
    private readonly IAuditLogDbContext _dbContext;

    public GetAllAuditLogsQueryHandler(ILogger<GetAllAuditLogsQueryHandler> logger, IAuditLogDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
    public async Task<PagedResponse<AuditResponse>> Handle(GetAllAuditLogsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching logs from database. PageNumber: {PageNumber}, PageSize: {PageSize}", request.PageNumber, request.PageSize);

        var logs =  _dbContext.AuditLogs.AsQueryable();

        if (request.UserId.HasValue)
        {
            logs = logs.Where(s => s.UserId == request.UserId);
        }

        if (!string.IsNullOrEmpty(request.Action))
        {
            logs = logs.Where(s => s.Action.Contains(request.Action));
        }

        if (!string.IsNullOrEmpty(request.EntityName))
        {
            logs = logs.Where(s => s.EntityName.Contains(request.EntityName));
        }

        if (!string.IsNullOrEmpty(request.ServiceName))
        {
            logs = logs.Where(s => s.ServiceName.Contains(request.ServiceName));
        }

        if (request.From.HasValue)
        {
            logs = logs.Where(s => s.OccurredAt >=request.From.Value.ToUniversalTime());
        }

        if (request.To.HasValue)
        {
            logs = logs.Where(s => s.OccurredAt <= request.To.Value.ToUniversalTime());
        }

        var totalCount = await logs.CountAsync(cancellationToken);
        var auditLogs = await logs
            .OrderBy(s => s.Id)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var items = auditLogs.Select(log => new AuditResponse
        {
            OccurredAt = log.OccurredAt,
            Id = log.Id,
            UserId = log.UserId!.Value,
            UserName = log.UserName ?? "Unknown",
            EntityId = log.EntityId!.Value,
            EntityName = log.EntityName,
            Action = log.Action,
            ServiceName = log.ServiceName,
            LogDetailsList = log.AuditLogDetails
        }).ToList();


        return new PagedResponse<AuditResponse>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };


    }
}