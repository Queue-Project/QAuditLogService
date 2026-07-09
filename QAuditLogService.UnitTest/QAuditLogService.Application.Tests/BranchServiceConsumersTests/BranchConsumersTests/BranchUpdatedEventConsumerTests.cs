using BranchService.Contracts.Events;
using BranchService.Contracts.Events.BranchEvents;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Application.Consumers.BranchService.BranchServiceConsumers;
using QAuditLogService.Contracts;
using QAuditLogService.Infrastructure.Persistence.Database;
using QAuditLogService.UnitTest.QAuditLogService.Application.Tests.Infrastructure;
using Shouldly;

namespace QAuditLogService.UnitTest.QAuditLogService.Application.Tests.BranchServiceConsumersTests.BranchConsumersTests
{
    public class BranchUpdatedEventConsumerTests
    {
        private readonly Mock<ILogger<BranchUpdatedEventConsumer>> _mockLogger;
        private readonly Mock<ConsumeContext<BranchUpdatedEvent>> _mockContext;
        private readonly AuditLogDbContext _context;
        private readonly BranchUpdatedEventConsumer _consumer;

        public BranchUpdatedEventConsumerTests()
        {
            _mockLogger = new Mock<ILogger<BranchUpdatedEventConsumer>>();
            _mockContext = new Mock<ConsumeContext<BranchUpdatedEvent>>();
            _context = TestDbContextFactory.Create();
            _consumer = new BranchUpdatedEventConsumer( _mockLogger.Object, _context);
        }


        [Fact]
        public async Task Consume_Should_Create_Audit_Log_For_Branch_When_Event_Received()
        {
            //Arrange
            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new BranchUpdatedEvent()
            {
                CompanyId = 1,
                BranchId = 1,
                BranchName = "Test Branch Name",
                EmailAddress = "test@gmail.com",
                Address = "Test Address",
                City = "Test City",
                IsActive = true,
                PhoneNumber = "+992923324252",
                OccuredAt = occuredAt,
                AuditData = new AuditData
                {
                    PerformedByUserId = 1,
                    PerformedByUserName = "systemAdmin",
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
            savedAuditLog.EntityId.ShouldBe(expectedEvent.BranchId);
            savedAuditLog.EntityName.ShouldBe("Branch");
            savedAuditLog.Action.ShouldBe("branch.updated");
            savedAuditLog.ServiceName.ShouldBe("BranchService");
            savedAuditLog.AuditLogDetails.ShouldNotBeEmpty();
            savedAuditLog.AuditLogDetails[0].PropertyName.ShouldBe(expectedEvent.AuditData.Changes[0].PropertyName);
            savedAuditLog.AuditLogDetails[0].OldValue.ShouldBe(expectedEvent.AuditData.Changes[0].OldValue);
            savedAuditLog.AuditLogDetails[0].NewValue.ShouldBe(expectedEvent.AuditData.Changes[0].NewValue);
        }
    }
}