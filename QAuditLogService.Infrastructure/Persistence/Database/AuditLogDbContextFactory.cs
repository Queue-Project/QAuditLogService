using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace QAuditLogService.Infrastructure.Persistence.Database;

public class AuditLogDbContextFactory: IDesignTimeDbContextFactory<AuditLogDbContext>
{
    public AuditLogDbContext CreateDbContext(string[] args)
    {
        var optionBuilder = new DbContextOptionsBuilder<AuditLogDbContext>();
        
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../QAuditLogService.API"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
        
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        optionBuilder.UseNpgsql(connectionString);
        return new AuditLogDbContext(optionBuilder.Options);
    }
}