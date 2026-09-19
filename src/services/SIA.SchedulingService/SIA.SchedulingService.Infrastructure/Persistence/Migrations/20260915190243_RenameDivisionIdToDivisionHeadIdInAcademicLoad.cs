using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIA.SchedulingService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameDivisionIdToDivisionHeadIdInAcademicLoad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DivisionId",
                table: "AcademicLoad",
                newName: "DivisionHeadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DivisionHeadId",
                table: "AcademicLoad",
                newName: "DivisionId");
        }
    }
}
