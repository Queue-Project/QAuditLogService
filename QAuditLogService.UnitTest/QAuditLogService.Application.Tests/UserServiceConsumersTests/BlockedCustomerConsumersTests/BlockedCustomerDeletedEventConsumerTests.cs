using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Application.Consumers.UserService.BlockedCustomerConsumers;
using QAuditLogService.Contracts;
using QAuditLogService.Infrastructure.Persistence.Database;
using QAuditLogService.UnitTest.QAuditLogService.Application.Tests.Infrastructure;
using QUserService.Contracts;
using QUserService.Contracts.Events.BlockedCustomerEvent;
using Shouldly;

namespace QAuditLogService.UnitTest.QAuditLogService.Application.Tests.UserServiceConsumersTests.
    BlockedCustomerConsumersTests;

public class BlockedCustomerDeletedEventConsumerTests
{
    private readonly Mock<ILogger<BlockedCustomerDeletedEventConsumer>> _mockLogger;
    private readonly Mock<ConsumeContext<BlockedCustomerDeletedEvent>> _mockContext;
    private readonly AuditLogDbContext _context;
    private readonly BlockedCustomerDeletedEventConsumer _consumer;

    public BlockedCustomerDeletedEventConsumerTests()
    {
        _mockLogger = new Mock<ILogger<BlockedCustomerDeletedEventConsumer>>();
        _mockContext = new Mock<ConsumeContext<BlockedCustomerDeletedEvent>>();
        _context = TestDbContextFactory.Create();
        _consumer = new BlockedCustomerDeletedEventConsumer(_mockLogger.Object, _context);
    }


    [Fact]
    public async Task Consume_Should_Create_Audit_Log_For_Blocked_Customer_When_Event_Received()
    {
        //Arrange
        var occuredAt = DateTime.UtcNow;


        var expectedEvent = new BlockedCustomerDeletedEvent
        {
            BlockedCustomerId = 1,
            CustomerId = 1,
            CompanyId = 1,
            BannedUntil = DateTime.UtcNow.AddMonths(1),
            DoesBanForever = false,
            Reason = "Test Reason",
            OccuredAt = occuredAt,
            AuditData = new AuditData
            {
                PerformedByUserId = 1,
                PerformedByUserName = "employee",
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
        savedAuditLog.EntityId.ShouldBe(expectedEvent.BlockedCustomerId);
        savedAuditLog.EntityName.ShouldBe("BlockedCustomer");
        savedAuditLog.Action.ShouldBe("blocked.customer.deleted");
        savedAuditLog.ServiceName.ShouldBe("UserService");
        savedAuditLog.AuditLogDetails.ShouldBeEmpty();
    }
}