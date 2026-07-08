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

public class ComplaintUpdatedEventConsumerTests
{
      private readonly Mock<ILogger<ComplaintUpdatedEventConsumer>> _mockLogger;
    private readonly Mock<ConsumeContext<ComplaintUpdatedEvent>> _mockContext;
    private readonly AuditLogDbContext _context;
    private readonly ComplaintUpdatedEventConsumer _consumer;

    public ComplaintUpdatedEventConsumerTests()
    {
        _mockLogger = new Mock<ILogger<ComplaintUpdatedEventConsumer>>();
        _mockContext = new Mock<ConsumeContext<ComplaintUpdatedEvent>>();
        _context = TestDbContextFactory.Create();
        _consumer = new ComplaintUpdatedEventConsumer(_mockLogger.Object, _context);
    }


    [Fact]
    public async Task Consume_Should_Create_Audit_Log_For_Complaint_Reviewed_Status_When_Event_Received()
    {
        //Arrange
        var occuredAt = DateTime.UtcNow;


        var expectedEvent = new ComplaintUpdatedEvent()
        {
            ComplaintId = 1,
            CustomerId = 1,
            EmployeeId = 1,
            QueueId = 1,
            ComplaintText = "Test Text",
            ResponseText = "Test Response Text",
            CurrentComplaintStatus = CurrentComplaintStatus.Reviewed,
            OccuredAt = occuredAt,
            AuditData = new AuditData
            {
                PerformedByUserId = 1,
                PerformedByUserName = "employee",
                Changes = new List<AuditEventLogDetails>
                {
                    new AuditEventLogDetails
                    {
                        PropertyName = "ComplaintStatus",
                        OldValue = "CurrentComplaintStatus.Pending",
                        NewValue = "CurrentComplaintStatus.Reviewed"
                    }
                }
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
        savedAuditLog.Action.ShouldBe("complaint.updated.to.reviewed");
        savedAuditLog.ServiceName.ShouldBe("QueueService");
        savedAuditLog.AuditLogDetails.ShouldNotBeEmpty();
        savedAuditLog.AuditLogDetails[0].PropertyName.ShouldBe(expectedEvent.AuditData.Changes[0].PropertyName);
        savedAuditLog.AuditLogDetails[0].OldValue.ShouldBe(expectedEvent.AuditData.Changes[0].OldValue);
        savedAuditLog.AuditLogDetails[0].NewValue.ShouldBe(expectedEvent.AuditData.Changes[0].NewValue);
    }
    
      [Fact]
    public async Task Consume_Should_Create_Audit_Log_For_Complaint_Resolved_Status_When_Event_Received()
    {
        //Arrange
        var occuredAt = DateTime.UtcNow;


        var expectedEvent = new ComplaintUpdatedEvent()
        {
            ComplaintId = 1,
            CustomerId = 1,
            EmployeeId = 1,
            QueueId = 1,
            ComplaintText = "Test Text",
            ResponseText = "Test Response Text",
            CurrentComplaintStatus = CurrentComplaintStatus.Resolved,
            OccuredAt = occuredAt,
            AuditData = new AuditData
            {
                PerformedByUserId = 1,
                PerformedByUserName = "employee",
                Changes = new List<AuditEventLogDetails>
                {
                    new AuditEventLogDetails
                    {
                        PropertyName = "ComplaintStatus",
                        OldValue = "CurrentComplaintStatus.Reviewed",
                        NewValue = "CurrentComplaintStatus.Resolved"
                    }
                }
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
        savedAuditLog.Action.ShouldBe("complaint.updated.to.resolved");
        savedAuditLog.ServiceName.ShouldBe("QueueService");
        savedAuditLog.AuditLogDetails.ShouldNotBeEmpty();
        savedAuditLog.AuditLogDetails[0].PropertyName.ShouldBe(expectedEvent.AuditData.Changes[0].PropertyName);
        savedAuditLog.AuditLogDetails[0].OldValue.ShouldBe(expectedEvent.AuditData.Changes[0].OldValue);
        savedAuditLog.AuditLogDetails[0].NewValue.ShouldBe(expectedEvent.AuditData.Changes[0].NewValue);
    }
}