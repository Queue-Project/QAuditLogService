using MassTransit;
using Microsoft.Extensions.Logging;
using QAuditLogService.Application.Interfaces;
using QAuditLogService.Domain.Models;
using QUserService.Contracts.Events.EmployeeEvent;

namespace QAuditLogService.Application.Consumers.UserService.EmployeeConsumers;

public class EmployeeDeletedEventConsumer : IConsumer<EmployeeDeletedEvent>
{
    private readonly ILogger<EmployeeDeletedEventConsumer> _logger;
    private readonly IAuditLogDbContext _dbContext;

    public EmployeeDeletedEventConsumer(ILogger<EmployeeDeletedEventConsumer> logger, IAuditLogDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }


    public async Task Consume(ConsumeContext<EmployeeDeletedEvent> context)
    {
        var request = context.Message;
        _logger.LogInformation("Creating audit log for Employee Entity ");

        var auditLog = new AuditLog
        {
            OccurredAt = request.OccurredAt.DateTime,
            UserId = request.AuditData!.PerformedByUserId,
            UserName = request.AuditData!.PerformedByUserName,
            EntityId = request.EmployeeId,
            EntityName = "Employee",
            Action = "employee.deleted",
            ServiceName = "UserService",
            AuditLogDetails = request.AuditData!.Changes
        };


        await _dbContext.AuditLogs.AddAsync(auditLog);
        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Audit log created with Id {Id}", auditLog.Id);
    }
}