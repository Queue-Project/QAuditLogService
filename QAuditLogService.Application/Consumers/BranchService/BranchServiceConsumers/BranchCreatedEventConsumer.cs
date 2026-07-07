using BranchService.Contracts.Events.BranchEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using QAuditLogService.Application.Interfaces;
using QAuditLogService.Domain.Models;

namespace QAuditLogService.Application.Consumers.BranchService.BranchServiceConsumers;

public class BranchCreatedEventConsumer: IConsumer<BranchCreatedEvent>
{
    private readonly ILogger<BranchCreatedEventConsumer> _logger;
    private readonly IAuditLogDbContext _dbContext;

    public BranchCreatedEventConsumer(ILogger<BranchCreatedEventConsumer> logger, IAuditLogDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<BranchCreatedEvent> context)
    {
        var request = context.Message;
        _logger.LogInformation("Creating audit log for Branch Entity ");

        var auditLog = new AuditLog
        {
            OccurredAt = request.OccuredAt.DateTime,
            UserId = request.AuditData!.PerformedByUserId,
            UserName = request.AuditData!.PerformedByUserName,
            EntityId = request.BranchId,
            EntityName = "Branch",
            Action = "branch.created",
            ServiceName = "BranchService",
            AuditLogDetails = request.AuditData!.Changes
        };


        await _dbContext.AuditLogs.AddAsync(auditLog);
        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Audit log created with Id {Id}", auditLog.Id);
    }
}