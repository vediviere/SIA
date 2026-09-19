using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIA.WorkflowService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewDecisions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DecidedAtUtc",
                table: "ReviewProcesses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DecidedBy",
                table: "ReviewProcesses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DecisionCorrelationId",
                table: "ReviewProcesses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Payload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OccurredAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastAttemptAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextAttemptAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeadLetteredAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    Error = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReviewObservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewProcessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetType = table.Column<int>(type: "int", nullable: false),
                    TargetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewObservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewObservations_ReviewProcesses_ReviewProcessId",
                        column: x => x.ReviewProcessId,
                        principalTable: "ReviewProcesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewProcesses_DecisionCorrelationId",
                table: "ReviewProcesses",
                column: "DecisionCorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_CorrelationId",
                table: "OutboxMessages",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_ProcessedAtUtc_DeadLetteredAtUtc_NextAttemptAtUtc",
                table: "OutboxMessages",
                columns: new[] { "ProcessedAtUtc", "DeadLetteredAtUtc", "NextAttemptAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewObservations_CorrelationId",
                table: "ReviewObservations",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewObservations_ReviewProcessId",
                table: "ReviewObservations",
                column: "ReviewProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewObservations_TargetType_TargetId",
                table: "ReviewObservations",
                columns: new[] { "TargetType", "TargetId" });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewObservations_TenantId_ReviewProcessId",
                table: "ReviewObservations",
                columns: new[] { "TenantId", "ReviewProcessId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutboxMessages");

            migrationBuilder.DropTable(
                name: "ReviewObservations");

            migrationBuilder.DropIndex(
                name: "IX_ReviewProcesses_DecisionCorrelationId",
                table: "ReviewProcesses");

            migrationBuilder.DropColumn(
                name: "DecidedAtUtc",
                table: "ReviewProcesses");

            migrationBuilder.DropColumn(
                name: "DecidedBy",
                table: "ReviewProcesses");

            migrationBuilder.DropColumn(
                name: "DecisionCorrelationId",
                table: "ReviewProcesses");
        }
    }
}
