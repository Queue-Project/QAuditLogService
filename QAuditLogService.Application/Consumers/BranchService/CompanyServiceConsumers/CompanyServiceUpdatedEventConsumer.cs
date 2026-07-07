using BranchService.Contracts.Events.CompanyServiceEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using QAuditLogService.Application.Interfaces;
using QAuditLogService.Domain.Models;

namespace QAuditLogService.Application.Consumers.BranchService.CompanyServiceConsumers;

public class CompanyServiceUpdatedEventConsumer: IConsumer<CompanyServiceUpdatedEvent>
{
    private readonly ILogger<CompanyServiceUpdatedEventConsumer> _logger;
    private readonly IAuditLogDbContext _dbContext;

    public CompanyServiceUpdatedEventConsumer(ILogger<CompanyServiceUpdatedEventConsumer> logger, IAuditLogDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<CompanyServiceUpdatedEvent> context)
    {
        var request = context.Message;
        _logger.LogInformation("Creating audit log for Company Service Entity ");

        var auditLog = new AuditLog
        {
            OccurredAt = request.OccuredAt.DateTime,
            UserId = request.AuditData!.PerformedByUserId,
            UserName = request.AuditData!.PerformedByUserName,
            EntityId = request.CompanyServiceId,
            EntityName = "CompanyService",
            Action = "company.service.updated",
            ServiceName = "BranchService",
            AuditLogDetails = request.AuditData!.Changes
        };


        await _dbContext.AuditLogs.AddAsync(auditLog);
        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Audit log created with Id {Id}", auditLog.Id);
    }
}