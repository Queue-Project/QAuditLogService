using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Application.UseCases.AuditLogs.Queries.GetAllAuditLogs;
using QAuditLogService.Infrastructure.Persistence.Database;
using QAuditLogService.UnitTest.QAuditLogService.Application.Tests.Infrastructure;
using Shouldly;

namespace QAuditLogService.UnitTest.QAuditLogService.Application.Tests.AuditLogsHandlerTests;

public class GetAllAuditLogsQueryHandlerTests
{
    private readonly Mock<ILogger<GetAllAuditLogsQueryHandler>> _mockLogger;
    private readonly AuditLogDbContext _context;
    private readonly GetAllAuditLogsQueryHandler _handler;

    public GetAllAuditLogsQueryHandlerTests()
    {
        _mockLogger = new Mock<ILogger<GetAllAuditLogsQueryHandler>>();
        _context = TestDbContextFactory.Create();
        _handler = new GetAllAuditLogsQueryHandler(_mockLogger.Object, _context);
    }

    [Fact]
    public async Task Handler_Should_Return_Audit_Logs_When_All_Parameters_Has_Value()
    {
        
        //Arrange
        var logs = TestDataSeeder.CreateAuditLogs();
        await _context.AuditLogs.AddRangeAsync(logs, CancellationToken.None);
        await _context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllAuditLogsQuery(1, "updated", "Customer", "UserService", new DateTime(2026, 07, 06),
            new DateTime(2026, 07, 09), 1, 10);
        
        //Act

        var result = await _handler.Handle(query, CancellationToken.None);
        
        //Assert

        result.ShouldNotBeNull();
        result.TotalCount.ShouldBe(3);
        result.PageNumber.ShouldBe(1);
        result.PageSize.ShouldBe(10);
        result.HasNextPage.ShouldBe(false);
        result.HasPreviousPage.ShouldBe(false);
        result.TotalPages.ShouldBe(1);
    }
}