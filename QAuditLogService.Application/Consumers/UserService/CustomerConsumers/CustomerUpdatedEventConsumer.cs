using MassTransit;
using Microsoft.Extensions.Logging;
using QAuditLogService.Application.Interfaces;
using QAuditLogService.Domain.Models;
using QUserService.Contracts.Events.CustomerEvent;

namespace QAuditLogService.Application.Consumers.UserService.CustomerConsumers;

public class CustomerUpdatedEventConsumer: IConsumer<CustomerUpdatedEvent>
{
    private readonly ILogger<CustomerUpdatedEventConsumer> _logger;
    private readonly IAuditLogDbContext _dbContext;

    public CustomerUpdatedEventConsumer(IAuditLogDbContext dbContext, ILogger<CustomerUpdatedEventConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CustomerUpdatedEvent> context)
    {
        var request = context.Message;
        _logger.LogInformation("Creating audit log for Employee Entity ");

        var auditLog = new AuditLog
        {
            OccurredAt = request.OccuredAt.DateTime,
            UserId = request.AuditData!.PerformedByUserId,
            UserName = request.AuditData!.PerformedByUserName,
            EntityId = request.CustomerId,
            EntityName = "Customer",
            Action = "customer.updated",
            ServiceName = "UserService",
            AuditLogDetails = request.AuditData!.Changes
        };


        await _dbContext.AuditLogs.AddAsync(auditLog);
        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Audit log created with Id {Id}", auditLog.Id);
    }
}