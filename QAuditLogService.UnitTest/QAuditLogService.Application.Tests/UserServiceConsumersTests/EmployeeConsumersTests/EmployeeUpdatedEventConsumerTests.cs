using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Application.Consumers.UserService.EmployeeConsumers;
using QAuditLogService.Contracts;
using QAuditLogService.Infrastructure.Persistence.Database;
using QAuditLogService.UnitTest.QAuditLogService.Application.Tests.Infrastructure;
using QUserService.Contracts;
using QUserService.Contracts.Events.EmployeeEvent;
using Shouldly;

namespace QAuditLogService.UnitTest.QAuditLogService.Application.Tests.UserServiceConsumersTests.EmployeeConsumersTests;

public class EmployeeUpdatedEventConsumerTests
{
    private readonly Mock<ILogger<EmployeeUpdatedEventConsumer>> _mockLogger;
    private readonly Mock<ConsumeContext<EmployeeUpdatedEvent>> _mockContext;
    private readonly AuditLogDbContext _context;
    private readonly EmployeeUpdatedEventConsumer _consumer;

    public EmployeeUpdatedEventConsumerTests()
    {
        _mockLogger = new Mock<ILogger<EmployeeUpdatedEventConsumer>>();
        _mockContext = new Mock<ConsumeContext<EmployeeUpdatedEvent>>();
        _context = TestDbContextFactory.Create();
        _consumer = new EmployeeUpdatedEventConsumer(_mockLogger.Object, _context);
    }


    [Fact]
    public async Task Consume_Should_Create_Audit_Log_For_Employee_When_Event_Received()
    {
        //Arrange
        var occuredAt = DateTime.UtcNow;


        var expectedEvent = new EmployeeUpdatedEvent
        {
            EmployeeId = 1,
            FirstName = "Test First Name",
            LastName = "Test Last Name",
            PhoneNumber = "+992923324252",
            Position = "Test Position",
            OccurredAt = occuredAt,
            AuditData = new AuditData
            {
                PerformedByUserId = 1,
                PerformedByUserName = "companyAdmin",
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
        savedAuditLog.OccurredAt.ShouldBe(expectedEvent.OccurredAt);
        savedAuditLog.UserId.ShouldBe(expectedEvent.AuditData.PerformedByUserId);
        savedAuditLog.UserName.ShouldBe(expectedEvent.AuditData.PerformedByUserName);
        savedAuditLog.EntityId.ShouldBe(expectedEvent.EmployeeId);
        savedAuditLog.EntityName.ShouldBe("Employee");
        savedAuditLog.Action.ShouldBe("employee.updated");
        savedAuditLog.ServiceName.ShouldBe("UserService");
        savedAuditLog.AuditLogDetails.ShouldNotBeEmpty();
        savedAuditLog.AuditLogDetails[0].PropertyName.ShouldBe(expectedEvent.AuditData.Changes[0].PropertyName);
        savedAuditLog.AuditLogDetails[0].OldValue.ShouldBe(expectedEvent.AuditData.Changes[0].OldValue);
        savedAuditLog.AuditLogDetails[0].NewValue.ShouldBe(expectedEvent.AuditData.Changes[0].NewValue);
    }
}