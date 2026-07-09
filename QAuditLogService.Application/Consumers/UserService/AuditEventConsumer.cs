using MassTransit;
using Microsoft.Extensions.Logging;
using QAuditLogService.Application.Interfaces;
using QAuditLogService.Contracts.AuditEvents;
using QAuditLogService.Domain.Models;

namespace QAuditLogService.Application.Consumers.UserService;

public class AuditEventConsumer: IConsumer<AuditEvent>
{
    private readonly ILogger<AuditEventConsumer> _logger;
    private readonly IAuditLogDbContext _dbContext;

    public AuditEventConsumer(ILogger<AuditEventConsumer> logger, IAuditLogDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<AuditEvent> context)
    {
        var request = context.Message;
        _logger.LogInformation("Creating audit log ");

        var auditLog = new AuditLog
        {
            OccurredAt = request.OccuredAt,
            UserId = request.UserId,
            UserName = request.UserName,
            EntityId = request.EntityId,
            EntityName = request.EntityName,
            Action = request.Action,
            ServiceName = request.ServiceName,
            AuditLogDetails = request.AuditLogDetails
        };


        await _dbContext.AuditLogs.AddAsync(auditLog);
        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Audit log created with Id {Id}", auditLog.Id);
    }
}