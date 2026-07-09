using MassTransit;
using Microsoft.Extensions.Logging;
using QAuditLogService.Application.Interfaces;
using QAuditLogService.Domain.Models;
using QContracts.Events.ComplaintEvents;

namespace QAuditLogService.Application.Consumers.QueueService.ComplaintConsumers;

public class ComplaintCreatedEventConsumer: IConsumer<ComplaintCreatedEvent>
{
    private readonly ILogger<ComplaintCreatedEventConsumer> _logger;
    private readonly IAuditLogDbContext _dbContext;

    public ComplaintCreatedEventConsumer(ILogger<ComplaintCreatedEventConsumer> logger, IAuditLogDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<ComplaintCreatedEvent> context)
    {
        var request = context.Message;
        _logger.LogInformation("Creating audit log for Complaint Entity ");

        var auditLog = new AuditLog
        {
            OccurredAt = request.OccuredAt,
            UserId = request.AuditData!.PerformedByUserId,
            UserName = request.AuditData!.PerformedByUserName,
            EntityId = request.ComplaintId,
            EntityName = "Complaint",
            Action = "complaint.created",
            ServiceName = "QueueService",
            AuditLogDetails = request.AuditData!.Changes
        };


        await _dbContext.AuditLogs.AddAsync(auditLog);
        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Audit log created with Id {Id}", auditLog.Id);
    }
}