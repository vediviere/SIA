using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIA.AcademicService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ACAD19_AddPrerequisiteSubjectId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PrerequisiteSubjectId",
                table: "StudyPlanSubjects",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudyPlanSubjects_PrerequisiteSubjectId",
                table: "StudyPlanSubjects",
                column: "PrerequisiteSubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyPlanSubjects_Subjects_PrerequisiteSubjectId",
                table: "StudyPlanSubjects",
                column: "PrerequisiteSubjectId",
                principalTable: "Subjects",
                principalColumn: "SubjectId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudyPlanSubjects_Subjects_PrerequisiteSubjectId",
                table: "StudyPlanSubjects");

            migrationBuilder.DropIndex(
                name: "IX_StudyPlanSubjects_PrerequisiteSubjectId",
                table: "StudyPlanSubjects");

            migrationBuilder.DropColumn(
                name: "PrerequisiteSubjectId",
                table: "StudyPlanSubjects");
        }
    }
}
