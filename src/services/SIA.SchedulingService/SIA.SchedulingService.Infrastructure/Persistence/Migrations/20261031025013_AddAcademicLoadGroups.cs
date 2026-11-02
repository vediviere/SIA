using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIA.SchedulingService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAcademicLoadGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AcademicLoadGroups",
                columns: table => new
                {
                    AcademicLoadGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcademicLoadId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Hours = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicLoadGroups", x => x.AcademicLoadGroupId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicLoadGroups_AcademicLoadId",
                table: "AcademicLoadGroups",
                column: "AcademicLoadId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicLoadGroups_TenantId_GroupId",
                table: "AcademicLoadGroups",
                columns: new[] { "TenantId", "GroupId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcademicLoadGroups");
        }
    }
}
