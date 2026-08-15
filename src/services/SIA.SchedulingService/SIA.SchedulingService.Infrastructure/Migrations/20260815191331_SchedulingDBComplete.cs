using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIA.SchedulingService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SchedulingDBComplete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AcademicLoad",
                columns: table => new
                {
                    AcademicLoadId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DivisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcademicPeriodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OfficialLetterNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProposedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClassHours = table.Column<int>(type: "int", nullable: false),
                    SupportHours = table.Column<int>(type: "int", nullable: false),
                    AssignmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicLoad", x => x.AcademicLoadId);
                });

            migrationBuilder.CreateTable(
                name: "Buildings",
                columns: table => new
                {
                    BuildingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buildings", x => x.BuildingId);
                });

            migrationBuilder.CreateTable(
                name: "ClassroomTypes",
                columns: table => new
                {
                    ClassroomTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassroomTypes", x => x.ClassroomTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EducationalProgramId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Shift = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.GroupId);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
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
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupportActivities",
                columns: table => new
                {
                    ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Activity = table.Column<string>(type: "varchar(255)", nullable: false),
                    Observation = table.Column<string>(type: "varchar(500)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportActivities", x => x.ActivityId);
                });

            migrationBuilder.CreateTable(
                name: "ClassroomLabs",
                columns: table => new
                {
                    ClassroomLabId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BuildingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassroomTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassroomLabs", x => x.ClassroomLabId);
                    table.CheckConstraint("CK_Classrooms_Capacity_Positive", "[Capacity] > 0");
                    table.ForeignKey(
                        name: "FK_ClassroomLabs_Buildings_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Buildings",
                        principalColumn: "BuildingId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassroomLabs_ClassroomTypes_ClassroomTypeId",
                        column: x => x.ClassroomTypeId,
                        principalTable: "ClassroomTypes",
                        principalColumn: "ClassroomTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AcademicOffering",
                columns: table => new
                {
                    OfferingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcademicLoadId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OfferingStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicOffering", x => x.OfferingId);
                    table.ForeignKey(
                        name: "FK_AcademicOffering_AcademicLoad_AcademicLoadId",
                        column: x => x.AcademicLoadId,
                        principalTable: "AcademicLoad",
                        principalColumn: "AcademicLoadId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AcademicOffering_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeachingSupportHours",
                columns: table => new
                {
                    SupportHourId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcademicLoadId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Hours = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeachingSupportHours", x => x.SupportHourId);
                    table.ForeignKey(
                        name: "FK_TeachingSupportHours_AcademicLoad_AcademicLoadId",
                        column: x => x.AcademicLoadId,
                        principalTable: "AcademicLoad",
                        principalColumn: "AcademicLoadId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeachingSupportHours_SupportActivities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "SupportActivities",
                        principalColumn: "ActivityId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClassSchedules",
                columns: table => new
                {
                    ClassScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OfferingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassroomLabId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcademicPeriodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Day = table.Column<string>(type: "varchar(20)", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassSchedules", x => x.ClassScheduleId);
                    table.ForeignKey(
                        name: "FK_ClassSchedules_ClassroomLabs_ClassroomLabId",
                        column: x => x.ClassroomLabId,
                        principalTable: "ClassroomLabs",
                        principalColumn: "ClassroomLabId",
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "IX_AcademicOffering_AcademicLoadId",
                table: "AcademicOffering",
                column: "AcademicLoadId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicOffering_GroupId",
                table: "AcademicOffering",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_TenantId_Code",
                table: "Buildings",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassroomLabs_BuildingId",
                table: "ClassroomLabs",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassroomLabs_ClassroomTypeId",
                table: "ClassroomLabs",
                column: "ClassroomTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassroomLabs_TenantId_Code",
                table: "ClassroomLabs",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassroomTypes_TenantId_Name",
                table: "ClassroomTypes",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassSchedules_ClassroomLabId",
                table: "ClassSchedules",
                column: "ClassroomLabId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_TenantId_EducationalProgramId_Shift_GroupName",
                table: "Groups",
                columns: new[] { "TenantId", "EducationalProgramId", "Shift", "GroupName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_CorrelationId",
                table: "OutboxMessages",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_ProcessedAtUtc",
                table: "OutboxMessages",
                column: "ProcessedAtUtc");

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

            migrationBuilder.CreateIndex(
                name: "IX_TeachingSupportHours_AcademicLoadId",
                table: "TeachingSupportHours",
                column: "AcademicLoadId");

            migrationBuilder.CreateIndex(
                name: "IX_TeachingSupportHours_ActivityId_AcademicLoadId",
                table: "TeachingSupportHours",
                columns: new[] { "ActivityId", "AcademicLoadId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcademicOffering");

            migrationBuilder.DropTable(
                name: "ClassSchedules");

            migrationBuilder.DropTable(
                name: "OutboxMessages");

            migrationBuilder.DropTable(
                name: "SupportSchedules");

            migrationBuilder.DropTable(
                name: "TeachingSupportHours");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "ClassroomLabs");

            migrationBuilder.DropTable(
                name: "AcademicLoad");

            migrationBuilder.DropTable(
                name: "SupportActivities");

            migrationBuilder.DropTable(
                name: "Buildings");

            migrationBuilder.DropTable(
                name: "ClassroomTypes");
        }
    }
}
