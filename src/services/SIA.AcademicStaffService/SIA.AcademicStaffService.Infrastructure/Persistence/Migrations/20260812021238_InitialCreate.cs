using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIA.AcademicStaffService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DivisionHead",
                columns: table => new
                {
                    DivisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EducationalProgramId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcademicDegree = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DivisionHead", x => x.DivisionId);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Payload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OccurredAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    Error = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teacher",
                columns: table => new
                {
                    TeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcademicDegree = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ProfessionalProfile = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ContractType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ContractHours = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teacher", x => x.TeacherId);
                    table.CheckConstraint("CK_Teacher_ContractHours_Positive", "[ContractHours] > 0");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DivisionHead_TenantId_EducationalProgramId_PersonId",
                table: "DivisionHead",
                columns: new[] { "TenantId", "EducationalProgramId", "PersonId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_CorrelationId",
                table: "OutboxMessage",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_ProcessedAtUtc",
                table: "OutboxMessage",
                column: "ProcessedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Teacher_TenantId_PersonId",
                table: "Teacher",
                columns: new[] { "TenantId", "PersonId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DivisionHead");

            migrationBuilder.DropTable(
                name: "OutboxMessage");

            migrationBuilder.DropTable(
                name: "Teacher");
        }
    }
}
