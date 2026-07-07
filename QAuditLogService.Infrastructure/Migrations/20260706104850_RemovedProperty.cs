using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QAuditLogService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovedProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "IsSuccess",
                table: "AuditLogs");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "AuditLogs",
                newName: "OccuredAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OccuredAt",
                table: "AuditLogs",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "AuditLogs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSuccess",
                table: "AuditLogs",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
