using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Application.Consumers.QueueService.ComplaintConsumers;
using QAuditLogService.Contracts;
using QAuditLogService.Infrastructure.Persistence.Database;
using QAuditLogService.UnitTest.QAuditLogService.Application.Tests.Infrastructure;
using QContracts.Enums;
using QContracts.Events;
using QContracts.Events.ComplaintEvents;
using Shouldly;

namespace QAuditLogService.UnitTest.QAuditLogService.Application.Tests.QueueServiceConsumersTests.ComplaintConsuemrsTests;

public class ComplaintCreatedEventConsumerTests
{
    private readonly Mock<ILogger<ComplaintCreatedEventConsumer>> _mockLogger;
    private readonly Mock<ConsumeContext<ComplaintCreatedEvent>> _mockContext;
    private readonly AuditLogDbContext _context;
    private readonly ComplaintCreatedEventConsumer _consumer;

    public ComplaintCreatedEventConsumerTests()
    {
        _mockLogger = new Mock<ILogger<ComplaintCreatedEventConsumer>>();
        _mockContext = new Mock<ConsumeContext<ComplaintCreatedEvent>>();
        _context = TestDbContextFactory.Create();
        _consumer = new ComplaintCreatedEventConsumer(_mockLogger.Object, _context);
    }


    [Fact]
    public async Task Consume_Should_Create_Audit_Log_For_Complaint_When_Event_Received()
    {
        //Arrange
        var occuredAt = DateTime.UtcNow;


        var expectedEvent = new ComplaintCreatedEvent()
        {
            ComplaintId = 1,
            CustomerId = 1,
            EmployeeId = 1,
            QueueId = 1,
            ComplaintText = "Test Text",
            CurrentComplaintStatus = CurrentComplaintStatus.Pending,
            OccuredAt = occuredAt,
            AuditData = new AuditData
            {
                PerformedByUserId = 1,
                PerformedByUserName = "customer",
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
        savedAuditLog.EntityId.ShouldBe(expectedEvent.ComplaintId);
        savedAuditLog.EntityName.ShouldBe("Complaint");
        savedAuditLog.Action.ShouldBe("complaint.created");
        savedAuditLog.ServiceName.ShouldBe("QueueService");
        savedAuditLog.AuditLogDetails.ShouldBeEmpty();
    }
}