using MassTransit;
using Microsoft.Extensions.Logging;
using QAuditLogService.Application.Interfaces;
using QAuditLogService.Domain.Models;
using QUserService.Contracts.Events.CustomerEvent;

namespace QAuditLogService.Application.Consumers.UserService.CustomerConsumers;

public class CustomerDeletedEventConsumer: IConsumer<CustomerDeletedEvent>
{
    private readonly ILogger<CustomerDeletedEventConsumer> _logger;
    private readonly IAuditLogDbContext _dbContext;

    public CustomerDeletedEventConsumer(ILogger<CustomerDeletedEventConsumer> logger, IAuditLogDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<CustomerDeletedEvent> context)
    {
        var request = context.Message;
        _logger.LogInformation("Creating audit log for Employee Entity ");

        var auditLog = new AuditLog
        {
            OccurredAt = request.OccuredAt,
            UserId = request.AuditData!.PerformedByUserId,
            UserName = request.AuditData!.PerformedByUserName,
            EntityId = request.CustomerId,
            EntityName = "Customer",
            Action = "customer.deleted",
            ServiceName = "UserService",
            AuditLogDetails = request.AuditData!.Changes
        };


        await _dbContext.AuditLogs.AddAsync(auditLog);
        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Audit log created with Id {Id}", auditLog.Id);
    }
}