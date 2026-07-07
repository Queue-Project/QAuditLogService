using BranchService.Contracts.Events.CompanyEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using QAuditLogService.Application.Interfaces;
using QAuditLogService.Domain.Models;


namespace QAuditLogService.Application.Consumers.BranchService.CompanyConsumers;

public class CompanyCreatedEventConsumer: IConsumer<CompanyCreatedEvent>
{
    private readonly ILogger<CompanyCreatedEventConsumer> _logger;
    private readonly IAuditLogDbContext _dbContext;
    

    public CompanyCreatedEventConsumer(IAuditLogDbContext dbContext, ILogger<CompanyCreatedEventConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task Consume(ConsumeContext<CompanyCreatedEvent> context)
    {
        var request = context.Message;
        _logger.LogInformation("Creating audit log for Company Entity " );

        var auditLog = new AuditLog
        {
            OccurredAt = request.OccuredAt.DateTime,
             
        };

        
       
        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Audit log created with Id {Id}", auditLog.Id);
    }
}