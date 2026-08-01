using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIA.AcademicService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSubjectsAndStudyPlanSubjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Subjects",
                newName: "SubjectId");

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "Subjects",
                type: "bit",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AddColumn<int>(
                name: "PracticeHours",
                table: "Subjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Semester",
                table: "Subjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "StudyPlanId",
                table: "Subjects",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "TheoryHours",
                table: "Subjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "StudyPlanSubjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudyPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Semester = table.Column<int>(type: "int", nullable: false),
                    Credits = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyPlanSubjects", x => x.Id);
                    table.CheckConstraint("CK_StudyPlanSubjects_Credits_Positive", "[Credits] > 0");
                    table.CheckConstraint("CK_StudyPlanSubjects_Semester_Positive", "[Semester] > 0");
                    table.ForeignKey(
                        name: "FK_StudyPlanSubjects_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Subjects_PracticeHours_NonNegative",
                table: "Subjects",
                sql: "[PracticeHours] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Subjects_Semester_Positive",
                table: "Subjects",
                sql: "[Semester] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Subjects_TheoryHours_NonNegative",
                table: "Subjects",
                sql: "[TheoryHours] >= 0");

            migrationBuilder.CreateIndex(
                name: "IX_StudyPlanSubjects_SubjectId",
                table: "StudyPlanSubjects",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyPlanSubjects_TenantId_StudyPlanId",
                table: "StudyPlanSubjects",
                columns: new[] { "TenantId", "StudyPlanId" });

            migrationBuilder.CreateIndex(
                name: "IX_StudyPlanSubjects_TenantId_SubjectId",
                table: "StudyPlanSubjects",
                columns: new[] { "TenantId", "SubjectId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudyPlanSubjects");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Subjects_PracticeHours_NonNegative",
                table: "Subjects");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Subjects_Semester_Positive",
                table: "Subjects");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Subjects_TheoryHours_NonNegative",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "PracticeHours",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "Semester",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "StudyPlanId",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "TheoryHours",
                table: "Subjects");

            migrationBuilder.RenameColumn(
                name: "SubjectId",
                table: "Subjects",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Subjects",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
