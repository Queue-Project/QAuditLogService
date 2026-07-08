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

public class EmployeeCreatedEventConsumerTests
{
    private readonly Mock<ILogger<EmployeeCreatedEventConsumer>> _mockLogger;
    private readonly Mock<ConsumeContext<EmployeeCreatedEvent>> _mockContext;
    private readonly AuditLogDbContext _context;
    private readonly EmployeeCreatedEventConsumer _consumer;

    public EmployeeCreatedEventConsumerTests()
    {
        _mockLogger = new Mock<ILogger<EmployeeCreatedEventConsumer>>();
        _mockContext = new Mock<ConsumeContext<EmployeeCreatedEvent>>();
        _context = TestDbContextFactory.Create();
        _consumer = new EmployeeCreatedEventConsumer( _mockLogger.Object, _context);
    }


    [Fact]
    public async Task Consume_Should_Create_Audit_Log_For_Employee_When_Event_Received()
    {
        //Arrange
        var occuredAt = DateTime.UtcNow;


        var expectedEvent = new EmployeeCreatedEvent
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
        savedAuditLog.EntityId.ShouldBe(expectedEvent.EmployeeId);
        savedAuditLog.EntityName.ShouldBe("Employee");
        savedAuditLog.Action.ShouldBe("employee.created");
        savedAuditLog.ServiceName.ShouldBe("UserService");
        savedAuditLog.AuditLogDetails.ShouldBeEmpty();
    }
}