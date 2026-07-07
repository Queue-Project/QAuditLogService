using MassTransit;
using Microsoft.Extensions.Logging;
using QAuditLogService.Application.Interfaces;
using QAuditLogService.Domain.Models;
using QContracts.Enums;
using QContracts.Events.ComplaintEvents;

namespace QAuditLogService.Application.Consumers.QueueService.ComplaintConsumers;

public class ComplaintUpdatedEventConsumer: IConsumer<ComplaintUpdatedEvent>
{
    private readonly ILogger<ComplaintUpdatedEventConsumer> _logger;
    private readonly IAuditLogDbContext _dbContext;

    public ComplaintUpdatedEventConsumer(ILogger<ComplaintUpdatedEventConsumer> logger, IAuditLogDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<ComplaintUpdatedEvent> context)
    {
        var request = context.Message;
        _logger.LogInformation("Creating audit log for Complaint Entity ");

        string action = "";
        if (request.CurrentComplaintStatus== CurrentComplaintStatus.Reviewed)
        {
            action = "complaint.updated.to.reviewed";
        }
        else if (request.CurrentComplaintStatus== CurrentComplaintStatus.Resolved)
        {
            action = "complaint.updated.to.resolved";
            
        }
        
        var auditLog = new AuditLog
        {
            OccurredAt = request.OccuredAt,
            UserId = request.AuditData!.PerformedByUserId,
            UserName = request.AuditData!.PerformedByUserName,
            EntityId = request.EmployeeId,
            EntityName = "Complaint",
            Action = action,
            ServiceName = "QueueService",
            AuditLogDetails = request.AuditData!.Changes
        };


        await _dbContext.AuditLogs.AddAsync(auditLog);
        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Audit log created with Id {Id}", auditLog.Id);
    }
}