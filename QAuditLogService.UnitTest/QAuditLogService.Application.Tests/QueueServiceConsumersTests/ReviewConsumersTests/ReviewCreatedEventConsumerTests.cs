using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Application.Consumers.QueueService.ReviewConsumers;
using QAuditLogService.Contracts;
using QAuditLogService.Infrastructure.Persistence.Database;
using QAuditLogService.UnitTest.QAuditLogService.Application.Tests.Infrastructure;
using QContracts.Events;
using QContracts.Events.ReviewEvents;
using Shouldly;

namespace QAuditLogService.UnitTest.QAuditLogService.Application.Tests.QueueServiceConsumersTests.ReviewConsumersTests;

public class ReviewCreatedEventConsumerTests
{
    private readonly Mock<ILogger<ReviewCreatedEventConsumer>> _mockLogger;
    private readonly Mock<ConsumeContext<ReviewCreatedEvent>> _mockContext;
    private readonly AuditLogDbContext _context;
    private readonly ReviewCreatedEventConsumer _consumer;

    public ReviewCreatedEventConsumerTests()
    {
        _mockLogger = new Mock<ILogger<ReviewCreatedEventConsumer>>();
        _mockContext = new Mock<ConsumeContext<ReviewCreatedEvent>>();
        _context = TestDbContextFactory.Create();
        _consumer = new ReviewCreatedEventConsumer(_mockLogger.Object, _context);
    }


    [Fact]
    public async Task Consume_Should_Create_Audit_Log_For_Review_When_Event_Received()
    {
        //Arrange
        var occuredAt = DateTime.UtcNow;


        var expectedEvent = new ReviewCreatedEvent()
        {
            ReviewId = 1,
            CustomerId = 1,
            EmployeeId = 1,
            QueueId = 1,
            Grade = 5,
            ReviewText = "Test Text",
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
        savedAuditLog.EntityId.ShouldBe(expectedEvent.ReviewId);
        savedAuditLog.EntityName.ShouldBe("Review");
        savedAuditLog.Action.ShouldBe("review.created");
        savedAuditLog.ServiceName.ShouldBe("QueueService");
        savedAuditLog.AuditLogDetails.ShouldBeEmpty();
    }
}