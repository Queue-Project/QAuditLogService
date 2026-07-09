using BranchService.Contracts.Events;
using BranchService.Contracts.Events.CompanyServiceEvents;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Application.Consumers.BranchService.CompanyServiceConsumers;
using QAuditLogService.Contracts;
using QAuditLogService.Infrastructure.Persistence.Database;
using QAuditLogService.UnitTest.QAuditLogService.Application.Tests.Infrastructure;
using Shouldly;

namespace QAuditLogService.UnitTest.QAuditLogService.Application.Tests.BranchServiceConsumersTests.
    CompanyServiceConsumersTests;

public class CompanyServiceDeletedEventConsumerTests
{
    private readonly Mock<ILogger<CompanyServiceDeletedEventConsumer>> _mockLogger;
    private readonly Mock<ConsumeContext<CompanyServiceDeletedEvent>> _mockContext;
    private readonly AuditLogDbContext _context;
    private readonly CompanyServiceDeletedEventConsumer _consumer;

    public CompanyServiceDeletedEventConsumerTests()
    {
        _mockLogger = new Mock<ILogger<CompanyServiceDeletedEventConsumer>>();
        _mockContext = new Mock<ConsumeContext<CompanyServiceDeletedEvent>>();
        _context = TestDbContextFactory.Create();
        _consumer = new CompanyServiceDeletedEventConsumer( _mockLogger.Object, _context);
    }


    [Fact]
    public async Task Consume_Should_Create_Audit_Log_For_Company_Service_When_Event_Received()
    {
        //Arrange
        var occuredAt = DateTimeOffset.UtcNow.DateTime;


        var expectedEvent = new CompanyServiceDeletedEvent()
        {
            CompanyId = 1,
            CompanyServiceId = 2,
            ServiceName = "Test Service Name",
            ServiceDescription = "Test Service Description",
            OccuredAt = occuredAt,
            AuditData = new AuditData
            {
                PerformedByUserId = 1,
                PerformedByUserName = "systemAdmin",
                Changes = new List<AuditEventLogDetails>()
            }
        };

        _mockContext.Setup(s => s.Message).Returns(expectedEvent);
        _mockContext.Setup(s => s.CancellationToken).Returns(CancellationToken.None);

        //Act
        await _consumer.Consume(_mockContext.Object);

        //Assert
        var savedAuditLog = await _context.AuditLogs.FirstOrDefaultAsync();

        savedAuditLog.ShouldNotBeNull();
        savedAuditLog.OccurredAt.ShouldBe(expectedEvent.OccuredAt);
        savedAuditLog.UserId.ShouldBe(expectedEvent.AuditData.PerformedByUserId);
        savedAuditLog.UserName.ShouldBe(expectedEvent.AuditData.PerformedByUserName);
        savedAuditLog.EntityId.ShouldBe(expectedEvent.CompanyServiceId);
        savedAuditLog.EntityName.ShouldBe("CompanyService");
        savedAuditLog.Action.ShouldBe("company.service.deleted");
        savedAuditLog.ServiceName.ShouldBe("BranchService");
        savedAuditLog.AuditLogDetails.ShouldBeEmpty();
    }
}