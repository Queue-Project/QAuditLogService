using MassTransit;
using Microsoft.Extensions.Logging;
using QAuditLogService.Application.Interfaces;
using QAuditLogService.Domain.Models;
using QUserService.Contracts.Events.BlockedCustomerEvent;

namespace QAuditLogService.Application.Consumers.UserService.BlockedCustomerConsumers;

public class BlockedCustomerCreatedEventConsumer: IConsumer<BlockedCustomerCreatedEvent>
{
    private readonly ILogger<BlockedCustomerCreatedEventConsumer> _logger;
    private readonly IAuditLogDbContext _dbContext;

    public BlockedCustomerCreatedEventConsumer(ILogger<BlockedCustomerCreatedEventConsumer> logger, IAuditLogDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<BlockedCustomerCreatedEvent> context)
    {
        
        var request = context.Message;
        _logger.LogInformation("Creating audit log for BlockedCustomer Entity ");

        var auditLog = new AuditLog
        {
            OccurredAt = request.OccuredAt.DateTime,
            UserId = request.AuditData!.PerformedByUserId,
            UserName = request.AuditData!.PerformedByUserName,
            EntityId = request.BlockedCustomerId,
            EntityName = "BlockedCustomer",
            Action = "blocked.customer.created",
            ServiceName = "UserService",
            AuditLogDetails = request.AuditData!.Changes
        };


        await _dbContext.AuditLogs.AddAsync(auditLog);
        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Audit log created with Id {Id}", auditLog.Id);
    }
}