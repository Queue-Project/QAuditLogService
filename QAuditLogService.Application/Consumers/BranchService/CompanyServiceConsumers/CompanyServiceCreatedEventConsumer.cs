using BranchService.Contracts.Events.CompanyServiceEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using QAuditLogService.Application.Interfaces;
using QAuditLogService.Domain.Models;

namespace QAuditLogService.Application.Consumers.BranchService.CompanyServiceConsumers;

public class CompanyServiceCreatedEventConsumer: IConsumer<CompanyServiceCreatedEvent>
{
    private readonly ILogger<CompanyServiceCreatedEventConsumer> _logger;
    private readonly IAuditLogDbContext _dbContext;

    public CompanyServiceCreatedEventConsumer(ILogger<CompanyServiceCreatedEventConsumer> logger, IAuditLogDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<CompanyServiceCreatedEvent> context)
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
            Action = "company.service.created",
            ServiceName = "BranchService",
            AuditLogDetails = request.AuditData!.Changes
        };


        await _dbContext.AuditLogs.AddAsync(auditLog);
        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Audit log created with Id {Id}", auditLog.Id);
    }
}