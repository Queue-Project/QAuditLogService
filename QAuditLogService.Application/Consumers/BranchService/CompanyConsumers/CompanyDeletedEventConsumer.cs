using BranchService.Contracts.Events.CompanyEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using QAuditLogService.Application.Interfaces;
using QAuditLogService.Domain.Models;

namespace QAuditLogService.Application.Consumers.BranchService.CompanyConsumers;

public class CompanyDeletedEventConsumer: IConsumer<CompanyDeletedEvent>
{
    private readonly ILogger<CompanyDeletedEventConsumer> _logger;
    private readonly IAuditLogDbContext _dbContext;

    public CompanyDeletedEventConsumer(ILogger<CompanyDeletedEventConsumer> logger, IAuditLogDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<CompanyDeletedEvent> context)
    {
        var request = context.Message;
        _logger.LogInformation("Creating audit log for Company Entity ");

        var auditLog = new AuditLog
        {
            OccurredAt = request.OccuredAt.DateTime,
            UserId = request.AuditData!.PerformedByUserId,
            UserName = request.AuditData!.PerformedByUserName,
            EntityId = request.CompanyId,
            EntityName = "Company",
            Action = "company.deleted",
            ServiceName = "BranchService",
            AuditLogDetails = request.AuditData!.Changes
        };


        await _dbContext.AuditLogs.AddAsync(auditLog);
        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Audit log created with Id {Id}", auditLog.Id);
    }
}