using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIA.WorkflowService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameWorkflowIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ReviewProcesses",
                newName: "ReviewProcessId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ReviewObservations",
                newName: "ReviewObservationId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "OutboxMessages",
                newName: "OutboxMessageId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "InboxMessages",
                newName: "InboxMessageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReviewProcessId",
                table: "ReviewProcesses",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ReviewObservationId",
                table: "ReviewObservations",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "OutboxMessageId",
                table: "OutboxMessages",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "InboxMessageId",
                table: "InboxMessages",
                newName: "Id");
        }
    }
}
