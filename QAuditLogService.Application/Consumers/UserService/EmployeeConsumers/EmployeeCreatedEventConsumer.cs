using MassTransit;
using Microsoft.Extensions.Logging;
using QAuditLogService.Application.Interfaces;
using QAuditLogService.Domain.Models;
using QUserService.Contracts.Events.EmployeeEvent;

namespace QAuditLogService.Application.Consumers.UserService.EmployeeConsumers;

public class EmployeeCreatedEventConsumer: IConsumer<EmployeeCreatedEvent>
{
    private readonly ILogger<EmployeeCreatedEventConsumer> _logger;
    private readonly IAuditLogDbContext _dbContext;

    public EmployeeCreatedEventConsumer(ILogger<EmployeeCreatedEventConsumer> logger, IAuditLogDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<EmployeeCreatedEvent> context)
    {
        var request = context.Message;
        _logger.LogInformation("Creating audit log for Employee Entity ");

        var auditLog = new AuditLog
        {
            OccurredAt = request.OccurredAt,
            UserId = request.AuditData!.PerformedByUserId,
            UserName = request.AuditData!.PerformedByUserName,
            EntityId = request.EmployeeId,
            EntityName = "Employee",
            Action = "employee.created",
            ServiceName = "UserService",
            AuditLogDetails = request.AuditData!.Changes
        };


        await _dbContext.AuditLogs.AddAsync(auditLog);
        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Audit log created with Id {Id}", auditLog.Id);
    }
}