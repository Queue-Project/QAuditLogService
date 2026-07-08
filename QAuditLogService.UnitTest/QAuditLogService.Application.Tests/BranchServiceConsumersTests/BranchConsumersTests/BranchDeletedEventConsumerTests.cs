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
    public class BranchDeletedEventConsumerTests
    {
        private readonly Mock<ILogger<BranchDeletedEventConsumer>> _mockLogger;
        private readonly Mock<ConsumeContext<BranchDeletedEvent>> _mockContext;
        private readonly AuditLogDbContext _context;
        private readonly BranchDeletedEventConsumer _consumer;

        public BranchDeletedEventConsumerTests()
        {
            _mockLogger = new Mock<ILogger<BranchDeletedEventConsumer>>();
            _mockContext = new Mock<ConsumeContext<BranchDeletedEvent>>();
            _context = TestDbContextFactory.Create();
            _consumer = new BranchDeletedEventConsumer( _mockLogger.Object, _context);
        }


        [Fact]
        public async Task Consume_Should_Create_Audit_Log_For_Branch_When_Event_Received()
        {
            //Arrange
            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new BranchDeletedEvent()
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
            savedAuditLog.EntityId.ShouldBe(expectedEvent.BranchId);
            savedAuditLog.EntityName.ShouldBe("Branch");
            savedAuditLog.Action.ShouldBe("branch.deleted");
            savedAuditLog.ServiceName.ShouldBe("BranchService");
            savedAuditLog.AuditLogDetails.ShouldBeEmpty();
            
        }
    }
}