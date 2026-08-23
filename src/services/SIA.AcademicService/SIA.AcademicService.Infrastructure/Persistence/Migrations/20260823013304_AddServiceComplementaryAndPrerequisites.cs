using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIA.AcademicService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceComplementaryAndPrerequisites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceComplementary",
                columns: table => new
                {
                    ComplementaryCreditId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudyPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<bool>(type: "bit", nullable: false),
                    Credit = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceComplementary", x => x.ComplementaryCreditId);
                    table.CheckConstraint("CK_ServiceComplementary_Credit_Positive", "[Credit] > 0");
                    table.ForeignKey(
                        name: "FK_ServiceComplementary_StudyPlans_StudyPlanId",
                        column: x => x.StudyPlanId,
                        principalTable: "StudyPlans",
                        principalColumn: "StudyPlanId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceComplementary_StudyPlanId",
                table: "ServiceComplementary",
                column: "StudyPlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceComplementary");
        }
    }
}
