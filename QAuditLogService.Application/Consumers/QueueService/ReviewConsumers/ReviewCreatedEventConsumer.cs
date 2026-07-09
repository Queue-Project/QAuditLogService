using MassTransit;
using Microsoft.Extensions.Logging;
using QAuditLogService.Application.Interfaces;
using QAuditLogService.Domain.Models;
using QContracts.Events.ReviewEvents;

namespace QAuditLogService.Application.Consumers.QueueService.ReviewConsumers;

public class ReviewCreatedEventConsumer : IConsumer<ReviewCreatedEvent>
{
    private readonly ILogger<ReviewCreatedEventConsumer> _logger;
    private readonly IAuditLogDbContext _dbContext;

    public ReviewCreatedEventConsumer(ILogger<ReviewCreatedEventConsumer> logger, IAuditLogDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }


    public async Task Consume(ConsumeContext<ReviewCreatedEvent> context)
    {
        var request = context.Message;
        _logger.LogInformation("Creating audit log for Review Entity ");

        var auditLog = new AuditLog
        {
            OccurredAt = request.OccuredAt,
            UserId = request.AuditData!.PerformedByUserId,
            UserName = request.AuditData!.PerformedByUserName,
            EntityId = request.EmployeeId,
            EntityName = "Review",
            Action = "review.created",
            ServiceName = "QueueService",
            AuditLogDetails = request.AuditData!.Changes
        };


        await _dbContext.AuditLogs.AddAsync(auditLog);
        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Audit log created with Id {Id}", auditLog.Id);
    }
}