using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIA.AcademicStaffService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherCompetencyEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TeacherCompetencies",
                columns: table => new
                {
                    TeacherCompetencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompetencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ValidatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Level = table.Column<string>(type: "varchar(50)", nullable: false),
                    Stage = table.Column<string>(type: "varchar(50)", nullable: false),
                    ValidateAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherCompetencies", x => x.TeacherCompetencyId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherCompetencies_TenantId_TeacherId_CompetencyId",
                table: "TeacherCompetencies",
                columns: new[] { "TenantId", "TeacherId", "CompetencyId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeacherCompetencies");
        }
    }
}
