using BranchService.Contracts.Events.BranchEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using QAuditLogService.Application.Interfaces;
using QAuditLogService.Domain.Models;

namespace QAuditLogService.Application.Consumers.BranchService.BranchServiceConsumers;

public class BranchUpdatedEventConsumer: IConsumer<BranchUpdatedEvent>
{
    private readonly ILogger<BranchUpdatedEventConsumer> _logger;
    private readonly IAuditLogDbContext _dbContext;

    public BranchUpdatedEventConsumer(ILogger<BranchUpdatedEventConsumer> logger, IAuditLogDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<BranchUpdatedEvent> context)
    {
        var request = context.Message;
        _logger.LogInformation("Creating audit log for Branch Entity ");

        var auditLog = new AuditLog
        {
            OccurredAt = request.OccuredAt,
            UserId = request.AuditData!.PerformedByUserId,
            UserName = request.AuditData!.PerformedByUserName,
            EntityId = request.BranchId,
            EntityName = "Branch",
            Action = "branch.updated",
            ServiceName = "BranchService",
            AuditLogDetails = request.AuditData!.Changes
        };


        await _dbContext.AuditLogs.AddAsync(auditLog);
        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Audit log created with Id {Id}", auditLog.Id);
    }
}