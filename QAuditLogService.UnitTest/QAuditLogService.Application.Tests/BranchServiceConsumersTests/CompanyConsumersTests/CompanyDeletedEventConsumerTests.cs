using BranchService.Contracts.Events;
using BranchService.Contracts.Events.CompanyEvents;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Application.Consumers.BranchService.CompanyConsumers;
using QAuditLogService.Contracts;
using QAuditLogService.Infrastructure.Persistence.Database;
using QAuditLogService.UnitTest.QAuditLogService.Application.Tests.Infrastructure;
using Shouldly;

namespace QAuditLogService.UnitTest.QAuditLogService.Application.Tests.BranchServiceConsumersTests.CompanyConsumersTests
{
    public class CompanyDeletedEventConsumerTests
    {
        private readonly Mock<ILogger<CompanyDeletedEventConsumer>> _mockLogger;
        private readonly Mock<ConsumeContext<CompanyDeletedEvent>> _mockContext;
        private readonly AuditLogDbContext _context;
        private readonly CompanyDeletedEventConsumer _consumer;

        public CompanyDeletedEventConsumerTests()
        {
            _mockLogger = new Mock<ILogger<CompanyDeletedEventConsumer>>();
            _mockContext = new Mock<ConsumeContext<CompanyDeletedEvent>>();
            _context = TestDbContextFactory.Create();
            _consumer = new CompanyDeletedEventConsumer(_mockLogger.Object, _context);
        }


        [Fact]
        public async Task Consume_Should_Create_Audit_Log_For_Company_When_Event_Received()
        {
            //Arrange
            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new CompanyDeletedEvent()
            {
                CompanyId = 1,
                CompanyName = "Test Company Name",
                EmailAddress = "test@gmail.com",
                Address = "Test Address",
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
            savedAuditLog.EntityId.ShouldBe(expectedEvent.CompanyId);
            savedAuditLog.EntityName.ShouldBe("Company");
            savedAuditLog.Action.ShouldBe("company.deleted");
            savedAuditLog.ServiceName.ShouldBe("BranchService");
            savedAuditLog.AuditLogDetails.ShouldBeEmpty();
            
        }
    }
}