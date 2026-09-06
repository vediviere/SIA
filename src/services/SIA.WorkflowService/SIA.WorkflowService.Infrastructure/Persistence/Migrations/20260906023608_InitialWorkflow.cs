using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIA.WorkflowService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InboxMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SourceService = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ReceivedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    Error = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReviewProcesses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcademicLoadProposalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DivisionHeadId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SubmittedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewProcesses", x => x.Id);
                    table.CheckConstraint("CK_ReviewProcesses_Version_Positive", "[Version] > 0");
                });

            migrationBuilder.CreateIndex(
                name: "IX_InboxMessages_CorrelationId",
                table: "InboxMessages",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_InboxMessages_ProcessedAtUtc",
                table: "InboxMessages",
                column: "ProcessedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewProcesses_CorrelationId",
                table: "ReviewProcesses",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewProcesses_TenantId_AcademicLoadProposalId_Version",
                table: "ReviewProcesses",
                columns: new[] { "TenantId", "AcademicLoadProposalId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewProcesses_TenantId_Status",
                table: "ReviewProcesses",
                columns: new[] { "TenantId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InboxMessages");

            migrationBuilder.DropTable(
                name: "ReviewProcesses");
        }
    }
}
