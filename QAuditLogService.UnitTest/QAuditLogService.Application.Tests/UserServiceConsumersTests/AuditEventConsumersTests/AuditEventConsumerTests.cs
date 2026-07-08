using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Application.Consumers.UserService;
using QAuditLogService.Contracts;
using QAuditLogService.Contracts.AuditEvents;
using QAuditLogService.Infrastructure.Persistence.Database;
using QAuditLogService.UnitTest.QAuditLogService.Application.Tests.Infrastructure;
using Shouldly;

namespace QAuditLogService.UnitTest.QAuditLogService.Application.Tests.UserServiceConsumersTests.
    AuditEventConsumersTests;

public class AuditEventConsumerTests
{
    private readonly Mock<ILogger<AuditEventConsumer>> _mockLogger;
    private readonly Mock<ConsumeContext<AuditEvent>> _mockContext;
    private readonly AuditLogDbContext _context;
    private readonly AuditEventConsumer _consumer;

    public AuditEventConsumerTests()
    {
        _mockLogger = new Mock<ILogger<AuditEventConsumer>>();
        _mockContext = new Mock<ConsumeContext<AuditEvent>>();
        _context = TestDbContextFactory.Create();
        _consumer = new AuditEventConsumer(_mockLogger.Object, _context);
    }


    [Fact]
    public async Task Consume_Should_Create_Audit_Log_For_Customer_When_Event_Received()
    {
        //Arrange
        var occuredAt = DateTime.UtcNow;


        var expectedEvent = new AuditEvent
        {
            OccuredAt = occuredAt,
            UserId = 1,
            UserName = "customer",
            EntityId = 1,
            EntityName = "UserEntity",
            ServiceName = "UserService",
            Action = "forgot.password",
            AuditLogDetails =
            [
                new AuditEventLogDetails
                {
                    PropertyName = "PasswordHash",
                    OldValue = "Test Old Password",
                    NewValue = "Test New Password"
                }
            ]
        };

        _mockContext.Setup(s => s.Message).Returns(expectedEvent);
        _mockContext.Setup(s => s.CancellationToken).Returns(CancellationToken.None);

        //Act
        await _consumer.Consume(_mockContext.Object);

        //Assert
        var savedAuditLog = await _context.AuditLogs.FirstOrDefaultAsync();

        savedAuditLog.ShouldNotBeNull();
        savedAuditLog.OccurredAt.ShouldBe(expectedEvent.OccuredAt);
        savedAuditLog.UserId.ShouldBe(expectedEvent.UserId);
        savedAuditLog.UserName.ShouldBe(expectedEvent.UserName);
        savedAuditLog.EntityId.ShouldBe(expectedEvent.EntityId);
        savedAuditLog.EntityName.ShouldBe(expectedEvent.EntityName);
        savedAuditLog.Action.ShouldBe(expectedEvent.Action);
        savedAuditLog.ServiceName.ShouldBe(expectedEvent.ServiceName);
        savedAuditLog.AuditLogDetails.ShouldNotBeEmpty();
        savedAuditLog.AuditLogDetails[0].PropertyName.ShouldBe(expectedEvent.AuditLogDetails[0].PropertyName);
        savedAuditLog.AuditLogDetails[0].OldValue.ShouldBe(expectedEvent.AuditLogDetails[0].OldValue);
        savedAuditLog.AuditLogDetails[0].NewValue.ShouldBe(expectedEvent.AuditLogDetails[0].NewValue);
    }
}