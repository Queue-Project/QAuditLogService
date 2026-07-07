using MassTransit;
using Microsoft.Extensions.Logging;
using QAuditLogService.Application.Interfaces;
using QAuditLogService.Domain.Models;
using QContracts.Events;
using QContracts.Events.Enums;

namespace QAuditLogService.Application.Consumers.QueueService.QueueConsumers;

public class QueueEventConsumer : IConsumer<QueueEvent>
{
    private readonly ILogger<QueueEventConsumer> _logger;
    private readonly IAuditLogDbContext _dbContext;

    public QueueEventConsumer(ILogger<QueueEventConsumer> logger, IAuditLogDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<QueueEvent> context)
    {
        var request = context.Message;
        _logger.LogInformation("Creating audit log for Queue Entity ");

        var auditLog = new AuditLog();

        if (request.EventType == QueueEventType.Created)
        {
            auditLog.OccurredAt = request.OccurredAt.DateTime;
            auditLog.UserId = request.AuditData!.PerformedByUserId;
            auditLog.UserName = request.AuditData!.PerformedByUserName;
            auditLog.EntityId = request.QueueId;
            auditLog.EntityName = "Queue";
            auditLog.Action = "queue.created.with.pending.status";
            auditLog.ServiceName = "QueueService";
            auditLog.AuditLogDetails = request.AuditData!.Changes;
        }

        if (request.EventType == QueueEventType.Updated)
        {
            switch (request.Status)
            {
                case UpdatedQueueStatus.Confirmed:
                    auditLog.OccurredAt = request.OccurredAt.DateTime;
                    auditLog.UserId = request.AuditData!.PerformedByUserId;
                    auditLog.UserName = request.AuditData!.PerformedByUserName;
                    auditLog.EntityId = request.QueueId;
                    auditLog.EntityName = "Queue";
                    auditLog.Action = "queue.updated.status.to.confirmed";
                    auditLog.ServiceName = "QueueService";
                    auditLog.AuditLogDetails = request.AuditData!.Changes;
                    break;
                case UpdatedQueueStatus.Completed:
                    auditLog.OccurredAt = request.OccurredAt.DateTime;
                    auditLog.UserId = request.AuditData!.PerformedByUserId;
                    auditLog.UserName = request.AuditData!.PerformedByUserName;
                    auditLog.EntityId = request.QueueId;
                    auditLog.EntityName = "Queue";
                    auditLog.Action = "queue.updated.status.to.completed";
                    auditLog.ServiceName = "QueueService";
                    auditLog.AuditLogDetails = request.AuditData!.Changes;
                    break;
                case UpdatedQueueStatus.CanceledByEmployee:
                    auditLog.OccurredAt = request.OccurredAt.DateTime;
                    auditLog.UserId = request.AuditData!.PerformedByUserId;
                    auditLog.UserName = request.AuditData!.PerformedByUserName;
                    auditLog.EntityId = request.QueueId;
                    auditLog.EntityName = "Queue";
                    auditLog.Action = "queue.updated.status.to.cancelled.by.employee";
                    auditLog.ServiceName = "QueueService";
                    auditLog.AuditLogDetails = request.AuditData!.Changes;
                    break;
                case UpdatedQueueStatus.CanceledByCustomer:
                    auditLog.OccurredAt = request.OccurredAt.DateTime;
                    auditLog.UserId = request.AuditData!.PerformedByUserId;
                    auditLog.UserName = request.AuditData!.PerformedByUserName;
                    auditLog.EntityId = request.QueueId;
                    auditLog.EntityName = "Queue";
                    auditLog.Action = "queue.updated.status.to.cancelled.by.customer";
                    auditLog.ServiceName = "QueueService";
                    auditLog.AuditLogDetails = request.AuditData!.Changes;
                    break;
            }
        }

        if (request.EventType == QueueEventType.StartingSoon)
        {
            return;
        }


        await _dbContext.AuditLogs.AddAsync(auditLog);
        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Audit log created with Id {Id}", auditLog.Id);
    }
}