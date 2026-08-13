using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIA.SchedulingService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSupportSchedulesEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SupportSchedules",
                columns: table => new
                {
                    SupportScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SupportHourId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassroomLabId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcademicPeriodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Day = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportSchedules", x => x.SupportScheduleId);
                    table.CheckConstraint("CK_SupportSchedules_TimeRange_Valid", "[StartTime] < [EndTime]");
                    table.ForeignKey(
                        name: "FK_SupportSchedules_ClassroomLabs_ClassroomLabId",
                        column: x => x.ClassroomLabId,
                        principalTable: "ClassroomLabs",
                        principalColumn: "ClassroomLabId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupportSchedules_AcademicPeriodId",
                table: "SupportSchedules",
                column: "AcademicPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportSchedules_ClassroomLabId",
                table: "SupportSchedules",
                column: "ClassroomLabId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportSchedules_SupportHourId",
                table: "SupportSchedules",
                column: "SupportHourId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportSchedules_TenantId_ClassroomLabId_Day_StartTime",
                table: "SupportSchedules",
                columns: new[] { "TenantId", "ClassroomLabId", "Day", "StartTime" });

            migrationBuilder.AddForeignKey(
                name: "FK_ClassroomLabs_Buildings_BuildingId",
                table: "ClassroomLabs",
                column: "BuildingId",
                principalTable: "Buildings",
                principalColumn: "BuildingId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassroomLabs_Buildings_BuildingId",
                table: "ClassroomLabs");

            migrationBuilder.DropTable(
                name: "SupportSchedules");
        }
    }
}
