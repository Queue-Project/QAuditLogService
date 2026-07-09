using Microsoft.EntityFrameworkCore;
using QAuditLogService.Infrastructure.Persistence.Database;

namespace QAuditLogService.UnitTest.QAuditLogService.Application.Tests.Infrastructure;

public static class TestDbContextFactory
{
    public static AuditLogDbContext Create()
    {
        var options = new DbContextOptionsBuilder<AuditLogDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new AuditLogDbContext(options);

        context.Database.EnsureCreated();

        return context;
    }
}