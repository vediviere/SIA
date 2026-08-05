using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIA.AcademicService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixStudyPlanSubjectRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudyPlanId",
                table: "Subjects");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "StudyPlanSubjects",
                newName: "StudyPlanSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyPlanSubjects_StudyPlanId",
                table: "StudyPlanSubjects",
                column: "StudyPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyPlanSubjects_StudyPlans_StudyPlanId",
                table: "StudyPlanSubjects",
                column: "StudyPlanId",
                principalTable: "StudyPlans",
                principalColumn: "StudyPlanId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudyPlanSubjects_StudyPlans_StudyPlanId",
                table: "StudyPlanSubjects");

            migrationBuilder.DropIndex(
                name: "IX_StudyPlanSubjects_StudyPlanId",
                table: "StudyPlanSubjects");

            migrationBuilder.RenameColumn(
                name: "StudyPlanSubjectId",
                table: "StudyPlanSubjects",
                newName: "Id");

            migrationBuilder.AddColumn<Guid>(
                name: "StudyPlanId",
                table: "Subjects",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
