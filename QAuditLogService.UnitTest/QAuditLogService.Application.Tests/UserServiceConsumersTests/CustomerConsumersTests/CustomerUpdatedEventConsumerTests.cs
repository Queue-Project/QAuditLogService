using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Application.Consumers.UserService.CustomerConsumers;
using QAuditLogService.Contracts;
using QAuditLogService.Infrastructure.Persistence.Database;
using QAuditLogService.UnitTest.QAuditLogService.Application.Tests.Infrastructure;
using QUserService.Contracts;
using QUserService.Contracts.Events.CustomerEvent;
using Shouldly;

namespace QAuditLogService.UnitTest.QAuditLogService.Application.Tests.UserServiceConsumersTests.CustomerConsumersTests;

public class CustomerUpdatedEventConsumerTests
{
    private readonly Mock<ILogger<CustomerUpdatedEventConsumer>> _mockLogger;
    private readonly Mock<ConsumeContext<CustomerUpdatedEvent>> _mockContext;
    private readonly AuditLogDbContext _context;
    private readonly CustomerUpdatedEventConsumer _consumer;

    public CustomerUpdatedEventConsumerTests()
    {
        _mockLogger = new Mock<ILogger<CustomerUpdatedEventConsumer>>();
        _mockContext = new Mock<ConsumeContext<CustomerUpdatedEvent>>();
        _context = TestDbContextFactory.Create();
        _consumer = new CustomerUpdatedEventConsumer(_context, _mockLogger.Object);
    }


    [Fact]
    public async Task Consume_Should_Create_Audit_Log_For_Customer_When_Event_Received()
    {
        //Arrange
        var occuredAt = DateTime.UtcNow;


        var expectedEvent = new CustomerUpdatedEvent
        {
            CustomerId = 1,
            FirstName = "Test First Name",
            LastName = "Test Last Name",
            PhoneNumber = "+992923324252",
            OccuredAt = occuredAt,
            AuditData = new AuditData
            {
                PerformedByUserId = 1,
                PerformedByUserName = "customer",
                Changes = new List<AuditEventLogDetails>
                {
                    new AuditEventLogDetails
                    {
                        PropertyName = "PhoneNumber",
                        OldValue = "+992923324251",
                        NewValue = "+992923324252"
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
        savedAuditLog.EntityId.ShouldBe(expectedEvent.CustomerId);
        savedAuditLog.EntityName.ShouldBe("Customer");
        savedAuditLog.Action.ShouldBe("customer.updated");
        savedAuditLog.ServiceName.ShouldBe("UserService");
        savedAuditLog.AuditLogDetails.ShouldNotBeEmpty();
        savedAuditLog.AuditLogDetails[0].PropertyName.ShouldBe(expectedEvent.AuditData.Changes[0].PropertyName);
        savedAuditLog.AuditLogDetails[0].OldValue.ShouldBe(expectedEvent.AuditData.Changes[0].OldValue);
        savedAuditLog.AuditLogDetails[0].NewValue.ShouldBe(expectedEvent.AuditData.Changes[0].NewValue);
    }
}