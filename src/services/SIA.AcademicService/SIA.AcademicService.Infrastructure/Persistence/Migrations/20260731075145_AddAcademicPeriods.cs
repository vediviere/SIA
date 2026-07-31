using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIA.AcademicService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAcademicPeriods : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AcademicPeriods",
                columns: table => new
                {
                    AcademicPeriodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    AcademicLoadProcessStartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    AcademicLoadProcessEndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EnrollmentProcessStartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EnrollmentProcessEndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PlanningSubmissionDate = table.Column<DateOnly>(type: "date", nullable: false),
                    FirstPartialGradeReportDate = table.Column<DateOnly>(type: "date", nullable: false),
                    SecondPartialGradeReportDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ThirdPartialGradeReportDate = table.Column<DateOnly>(type: "date", nullable: false),
                    FinalMinutesSubmissionDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicPeriods", x => x.AcademicPeriodId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicPeriods_TenantId_Code",
                table: "AcademicPeriods",
                columns: new[] { "TenantId", "Code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcademicPeriods");
        }
    }
}
