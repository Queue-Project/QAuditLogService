using MassTransit;
using Microsoft.Extensions.Logging;
using QAuditLogService.Application.Interfaces;
using QAuditLogService.Domain.Models;
using QUserService.Contracts.Events.CustomerEvent;

namespace QAuditLogService.Application.Consumers.UserService.CustomerConsumers;

public class CustomerCreatedEventConsumer : IConsumer<CustomerCreatedEvent>
{
    private readonly ILogger<CustomerCreatedEventConsumer> _logger;
    private readonly IAuditLogDbContext _dbContext;

    public CustomerCreatedEventConsumer(IAuditLogDbContext dbContext, ILogger<CustomerCreatedEventConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CustomerCreatedEvent> context)
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
            Action = "customer.created",
            ServiceName = "UserService",
            AuditLogDetails = request.AuditData!.Changes
        };


        await _dbContext.AuditLogs.AddAsync(auditLog);
        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Audit log created with Id {Id}", auditLog.Id);
    }
}