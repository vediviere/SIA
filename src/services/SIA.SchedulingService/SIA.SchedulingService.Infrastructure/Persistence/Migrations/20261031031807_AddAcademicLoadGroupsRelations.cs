using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIA.SchedulingService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAcademicLoadGroupsRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AcademicLoadGroups_GroupId",
                table: "AcademicLoadGroups",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicLoadGroups_AcademicLoad_AcademicLoadId",
                table: "AcademicLoadGroups",
                column: "AcademicLoadId",
                principalTable: "AcademicLoad",
                principalColumn: "AcademicLoadId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicLoadGroups_Groups_GroupId",
                table: "AcademicLoadGroups",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcademicLoadGroups_AcademicLoad_AcademicLoadId",
                table: "AcademicLoadGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_AcademicLoadGroups_Groups_GroupId",
                table: "AcademicLoadGroups");

            migrationBuilder.DropIndex(
                name: "IX_AcademicLoadGroups_GroupId",
                table: "AcademicLoadGroups");
        }
    }
}
