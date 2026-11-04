using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIA.AcademicStaffService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherCompetencyForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TeacherCompetencies_TeacherId",
                table: "TeacherCompetencies",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherCompetencies_Teachers_TeacherId",
                table: "TeacherCompetencies",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "TeacherId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeacherCompetencies_Teachers_TeacherId",
                table: "TeacherCompetencies");

            migrationBuilder.DropIndex(
                name: "IX_TeacherCompetencies_TeacherId",
                table: "TeacherCompetencies");
        }
    }
}
