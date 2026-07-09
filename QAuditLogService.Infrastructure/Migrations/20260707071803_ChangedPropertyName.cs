using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QAuditLogService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangedPropertyName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OccuredAt",
                table: "AuditLogs",
                newName: "OccurredAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OccurredAt",
                table: "AuditLogs",
                newName: "OccuredAt");
        }
    }
}
