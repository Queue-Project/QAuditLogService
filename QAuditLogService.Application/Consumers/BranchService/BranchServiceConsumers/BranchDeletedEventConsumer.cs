using BranchService.Contracts.Events.BranchEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using QAuditLogService.Application.Interfaces;
using QAuditLogService.Domain.Models;

namespace QAuditLogService.Application.Consumers.BranchService.BranchServiceConsumers;

public class BranchDeletedEventConsumer: IConsumer<BranchDeletedEvent>
{
    private readonly ILogger<BranchDeletedEventConsumer> _logger;
    private readonly IAuditLogDbContext _dbContext;

    public BranchDeletedEventConsumer(ILogger<BranchDeletedEventConsumer> logger, IAuditLogDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<BranchDeletedEvent> context)
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
            Action = "branch.deleted",
            ServiceName = "BranchService",
            AuditLogDetails = request.AuditData!.Changes
        };


        await _dbContext.AuditLogs.AddAsync(auditLog);
        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Audit log created with Id {Id}", auditLog.Id);
    }
}