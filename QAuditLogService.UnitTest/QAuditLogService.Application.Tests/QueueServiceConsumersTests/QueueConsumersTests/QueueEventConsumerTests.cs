using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Application.Consumers.QueueService.QueueConsumers;
using QAuditLogService.Contracts;
using QAuditLogService.Infrastructure.Persistence.Database;
using QAuditLogService.UnitTest.QAuditLogService.Application.Tests.Infrastructure;
using QContracts.Events;
using QContracts.Events.Enums;
using Shouldly;

namespace QAuditLogService.UnitTest.QAuditLogService.Application.Tests.QueueServiceConsumersTests.QueueConsumersTests;

public class QueueEventConsumerTests
{
     private readonly Mock<ILogger<QueueEventConsumer>> _mockLogger;
    private readonly Mock<ConsumeContext<QueueEvent>> _mockContext;
    private readonly AuditLogDbContext _context;
    private readonly QueueEventConsumer _consumer;

    public QueueEventConsumerTests()
    {
        _mockLogger = new Mock<ILogger<QueueEventConsumer>>();
        _mockContext = new Mock<ConsumeContext<QueueEvent>>();
        _context = TestDbContextFactory.Create();
        _consumer = new QueueEventConsumer(_mockLogger.Object, _context);
    }


    [Fact]
    public async Task Consume_Should_Create_Audit_Log_For_Queue_Created_When_Event_Received()
    {
        //Arrange
        var occuredAt = DateTime.UtcNow;


        var expectedEvent = new QueueEvent()
        {
            QueueId = 1,
            CustomerId = 1,
            EmployeeId = 1,
            CompanyId = 1,
            Email = "test@gmail.com",
            StartTime = DateTimeOffset.UtcNow,
            EndTime = null,
            CancelReason = null,
            EventType = QueueEventType.Created,
            Status = null,
            OccurredAt = occuredAt,
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
        savedAuditLog.OccurredAt.ShouldBe(expectedEvent.OccurredAt);
        savedAuditLog.UserId.ShouldBe(expectedEvent.AuditData.PerformedByUserId);
        savedAuditLog.UserName.ShouldBe(expectedEvent.AuditData.PerformedByUserName);
        savedAuditLog.EntityId.ShouldBe(expectedEvent.QueueId);
        savedAuditLog.EntityName.ShouldBe("Queue");
        savedAuditLog.Action.ShouldBe("queue.created.with.pending.status");
        savedAuditLog.ServiceName.ShouldBe("QueueService");
        savedAuditLog.AuditLogDetails.ShouldBeEmpty();
    }
    
    [Fact]
    public async Task Consume_Should_Create_Audit_Log_For_Queue_Updated_Confirmed_Status_When_Event_Received()
    {
        //Arrange
        var occuredAt = DateTime.UtcNow;


        var expectedEvent = new QueueEvent()
        {
            QueueId = 1,
            CustomerId = 1,
            EmployeeId = 1,
            CompanyId = 1,
            Email = "test@gmail.com",
            StartTime = DateTimeOffset.UtcNow,
            EndTime = DateTimeOffset.UtcNow.AddHours(1),
            CancelReason = null,
            EventType = QueueEventType.Updated,
            Status = UpdatedQueueStatus.Confirmed,
            OccurredAt = occuredAt,
            AuditData = new AuditData
            {
                PerformedByUserId = 1,
                PerformedByUserName = "employee",
                Changes = new List<AuditEventLogDetails>
                {
                    new AuditEventLogDetails
                    {
                        PropertyName = "Status",
                        OldValue = "UpdatedQueueStatus.Pending",
                        NewValue = "UpdatedQueueStatus.Confirmed"
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
        savedAuditLog.OccurredAt.ShouldBe(expectedEvent.OccurredAt);
        savedAuditLog.UserId.ShouldBe(expectedEvent.AuditData.PerformedByUserId);
        savedAuditLog.UserName.ShouldBe(expectedEvent.AuditData.PerformedByUserName);
        savedAuditLog.EntityId.ShouldBe(expectedEvent.QueueId);
        savedAuditLog.EntityName.ShouldBe("Queue");
        savedAuditLog.Action.ShouldBe("queue.updated.status.to.confirmed");
        savedAuditLog.ServiceName.ShouldBe("QueueService");
        savedAuditLog.AuditLogDetails.ShouldNotBeEmpty();
        savedAuditLog.AuditLogDetails[0].PropertyName.ShouldBe(expectedEvent.AuditData.Changes[0].PropertyName);
        savedAuditLog.AuditLogDetails[0].OldValue.ShouldBe(expectedEvent.AuditData.Changes[0].OldValue);
        savedAuditLog.AuditLogDetails[0].NewValue.ShouldBe(expectedEvent.AuditData.Changes[0].NewValue);
    }
    
    [Fact]
    public async Task Consume_Should_Create_Audit_Log_For_Queue_Updated_Completed_Status_When_Event_Received()
    {
        //Arrange
        var occuredAt = DateTime.UtcNow;


        var expectedEvent = new QueueEvent()
        {
            QueueId = 1,
            CustomerId = 1,
            EmployeeId = 1,
            CompanyId = 1,
            Email = "test@gmail.com",
            StartTime = DateTimeOffset.UtcNow,
            EndTime = DateTimeOffset.UtcNow.AddHours(1),
            CancelReason = null,
            EventType = QueueEventType.Updated,
            Status = UpdatedQueueStatus.Completed,
            OccurredAt = occuredAt,
            AuditData = new AuditData
            {
                PerformedByUserId = 1,
                PerformedByUserName = "employee",
                Changes = new List<AuditEventLogDetails>
                {
                    new AuditEventLogDetails
                    {
                        PropertyName = "Status",
                        OldValue = "UpdatedQueueStatus.Confirmed",
                        NewValue = "UpdatedQueueStatus.Completed"
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
        savedAuditLog.OccurredAt.ShouldBe(expectedEvent.OccurredAt);
        savedAuditLog.UserId.ShouldBe(expectedEvent.AuditData.PerformedByUserId);
        savedAuditLog.UserName.ShouldBe(expectedEvent.AuditData.PerformedByUserName);
        savedAuditLog.EntityId.ShouldBe(expectedEvent.QueueId);
        savedAuditLog.EntityName.ShouldBe("Queue");
        savedAuditLog.Action.ShouldBe("queue.updated.status.to.completed");
        savedAuditLog.ServiceName.ShouldBe("QueueService");
        savedAuditLog.AuditLogDetails.ShouldNotBeEmpty();
        savedAuditLog.AuditLogDetails[0].PropertyName.ShouldBe(expectedEvent.AuditData.Changes[0].PropertyName);
        savedAuditLog.AuditLogDetails[0].OldValue.ShouldBe(expectedEvent.AuditData.Changes[0].OldValue);
        savedAuditLog.AuditLogDetails[0].NewValue.ShouldBe(expectedEvent.AuditData.Changes[0].NewValue);
    }
    
     [Fact]
    public async Task Consume_Should_Create_Audit_Log_For_Queue_Updated_Cancelled_By_Employee_Status_When_Event_Received()
    {
        //Arrange
        var occuredAt = DateTime.UtcNow;


        var expectedEvent = new QueueEvent()
        {
            QueueId = 1,
            CustomerId = 1,
            EmployeeId = 1,
            CompanyId = 1,
            Email = "test@gmail.com",
            StartTime = DateTimeOffset.UtcNow,
            EndTime = null,
            CancelReason = "Test Text",
            EventType = QueueEventType.Updated,
            Status = UpdatedQueueStatus.CanceledByEmployee,
            OccurredAt = occuredAt,
            AuditData = new AuditData
            {
                PerformedByUserId = 1,
                PerformedByUserName = "employee",
                Changes = new List<AuditEventLogDetails>
                {
                    new AuditEventLogDetails
                    {
                        PropertyName = "Status",
                        OldValue = "UpdatedQueueStatus.Pending",
                        NewValue = "UpdatedQueueStatus.CancelledByEmployee"
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
        savedAuditLog.OccurredAt.ShouldBe(expectedEvent.OccurredAt);
        savedAuditLog.UserId.ShouldBe(expectedEvent.AuditData.PerformedByUserId);
        savedAuditLog.UserName.ShouldBe(expectedEvent.AuditData.PerformedByUserName);
        savedAuditLog.EntityId.ShouldBe(expectedEvent.QueueId);
        savedAuditLog.EntityName.ShouldBe("Queue");
        savedAuditLog.Action.ShouldBe("queue.updated.status.to.cancelled.by.employee");
        savedAuditLog.ServiceName.ShouldBe("QueueService");
        savedAuditLog.AuditLogDetails.ShouldNotBeEmpty();
        savedAuditLog.AuditLogDetails[0].PropertyName.ShouldBe(expectedEvent.AuditData.Changes[0].PropertyName);
        savedAuditLog.AuditLogDetails[0].OldValue.ShouldBe(expectedEvent.AuditData.Changes[0].OldValue);
        savedAuditLog.AuditLogDetails[0].NewValue.ShouldBe(expectedEvent.AuditData.Changes[0].NewValue);
    }
    
      [Fact]
    public async Task Consume_Should_Create_Audit_Log_For_Queue_Updated_Cancelled_By_Customer_Status_When_Event_Received()
    {
        //Arrange
        var occuredAt = DateTime.UtcNow;


        var expectedEvent = new QueueEvent()
        {
            QueueId = 1,
            CustomerId = 1,
            EmployeeId = 1,
            CompanyId = 1,
            Email = "test@gmail.com",
            StartTime = DateTimeOffset.UtcNow,
            EndTime = null,
            CancelReason = "Test Text",
            EventType = QueueEventType.Updated,
            Status = UpdatedQueueStatus.CanceledByCustomer,
            OccurredAt = occuredAt,
            AuditData = new AuditData
            {
                PerformedByUserId = 1,
                PerformedByUserName = "customer",
                Changes = new List<AuditEventLogDetails>
                {
                    new AuditEventLogDetails
                    {
                        PropertyName = "Status",
                        OldValue = "UpdatedQueueStatus.Pending",
                        NewValue = "UpdatedQueueStatus.CancelledByCustomer"
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
        savedAuditLog.OccurredAt.ShouldBe(expectedEvent.OccurredAt);
        savedAuditLog.UserId.ShouldBe(expectedEvent.AuditData.PerformedByUserId);
        savedAuditLog.UserName.ShouldBe(expectedEvent.AuditData.PerformedByUserName);
        savedAuditLog.EntityId.ShouldBe(expectedEvent.QueueId);
        savedAuditLog.EntityName.ShouldBe("Queue");
        savedAuditLog.Action.ShouldBe("queue.updated.status.to.cancelled.by.customer");
        savedAuditLog.ServiceName.ShouldBe("QueueService");
        savedAuditLog.AuditLogDetails.ShouldNotBeEmpty();
        savedAuditLog.AuditLogDetails[0].PropertyName.ShouldBe(expectedEvent.AuditData.Changes[0].PropertyName);
        savedAuditLog.AuditLogDetails[0].OldValue.ShouldBe(expectedEvent.AuditData.Changes[0].OldValue);
        savedAuditLog.AuditLogDetails[0].NewValue.ShouldBe(expectedEvent.AuditData.Changes[0].NewValue);
    }
    
       [Fact]
    public async Task Consume_Should_Not_Create_Audit_Log_For_Queue_Starting_Soon_When_Event_Received()
    {
        //Arrange
        var occuredAt = DateTime.UtcNow;


        var expectedEvent = new QueueEvent()
        {
            QueueId = 1,
            CustomerId = 1,
            EmployeeId = 1,
            CompanyId = 1,
            Email = "test@gmail.com",
            StartTime = DateTimeOffset.UtcNow,
            EndTime = DateTimeOffset.UtcNow.AddHours(1),
            CancelReason = null,
            EventType = QueueEventType.StartingSoon,
            Status = null,
            OccurredAt = occuredAt,
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

        savedAuditLog.ShouldBeNull();
        
       
    }
}