using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIA.SchedulingService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProgramId",
                table: "Groups",
                newName: "EducationalProgramId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Groups",
                newName: "GroupName");

            migrationBuilder.RenameIndex(
                name: "IX_Groups_TenantId_ProgramId_Shift_Name",
                table: "Groups",
                newName: "IX_Groups_TenantId_EducationalProgramId_Shift_GroupName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GroupName",
                table: "Groups",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "EducationalProgramId",
                table: "Groups",
                newName: "ProgramId");

            migrationBuilder.RenameIndex(
                name: "IX_Groups_TenantId_EducationalProgramId_Shift_GroupName",
                table: "Groups",
                newName: "IX_Groups_TenantId_ProgramId_Shift_Name");
        }
    }
}
