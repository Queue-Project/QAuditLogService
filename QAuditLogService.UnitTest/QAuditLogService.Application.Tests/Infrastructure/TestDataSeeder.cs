using QAuditLogService.Contracts;
using QAuditLogService.Domain.Models;

namespace QAuditLogService.UnitTest.QAuditLogService.Application.Tests.Infrastructure;

public static class TestDataSeeder
{
    public static List<AuditLog> CreateAuditLogs()
    {
        return new List<AuditLog>
        {
            new AuditLog
            {
                Id = 1,
                OccurredAt = new DateTimeOffset(new DateTime(2026, 07, 06, 09, 09, 10)),
                UserId = 1,
                UserName = "customer",
                EntityId = 1,
                EntityName = "Customer",
                ServiceName = "UserService",
                Action = "customer.updated",
                AuditLogDetails =
                [
                    new AuditEventLogDetails
                    {
                        PropertyName = "PasswordHash",
                        OldValue = "Test Old Password",
                        NewValue = "Test New Password"
                    }
                ]
            },
            new AuditLog
            {
                Id = 2,
                OccurredAt = new DateTimeOffset(new DateTime(2026, 07, 07, 12, 09, 10)),
                UserId = 1,
                UserName = "customer",
                EntityId = 1,
                EntityName = "Customer",
                ServiceName = "UserService",
                Action = "customer.updated",
                AuditLogDetails =
                [
                    new AuditEventLogDetails
                    {
                        PropertyName = "PasswordHash",
                        OldValue = "Test Old Password",
                        NewValue = "Test New Password"
                    }
                ]
            },
            new AuditLog
            {
                Id = 3,
                OccurredAt = new DateTimeOffset(new DateTime(2026, 07, 08, 15, 09, 10)),
                UserId = 1,
                UserName = "customer",
                EntityId = 1,
                EntityName = "Customer",
                ServiceName = "UserService",
                Action = "customer.updated",
                AuditLogDetails =
                [
                    new AuditEventLogDetails
                    {
                        PropertyName = "PasswordHash",
                        OldValue = "Test Old Password",
                        NewValue = "Test New Password"
                    }
                ]
            },
        };
    }
}