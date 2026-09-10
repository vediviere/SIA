using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIA.SchedulingService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAcademicLoadProposals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TeachingSupportHours_ActivityId_AcademicLoadId",
                table: "TeachingSupportHours");

            migrationBuilder.AddColumn<int>(
                name: "ClassHours",
                table: "AcademicOffering",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ProposalId",
                table: "AcademicLoad",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "AcademicLoadProposals",
                columns: table => new
                {
                    ProposalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EducationalProgramId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcademicPeriodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DivisionHeadId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProposalStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicLoadProposals", x => x.ProposalId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeachingSupportHours_ActivityId",
                table: "TeachingSupportHours",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_TeachingSupportHours_TenantId_AcademicLoadId_ActivityId",
                table: "TeachingSupportHours",
                columns: new[] { "TenantId", "AcademicLoadId", "ActivityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AcademicOffering_TenantId_GroupId_SubjectId",
                table: "AcademicOffering",
                columns: new[] { "TenantId", "GroupId", "SubjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AcademicLoad_ProposalId",
                table: "AcademicLoad",
                column: "ProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicLoad_TenantId_TeacherId_AcademicPeriodId",
                table: "AcademicLoad",
                columns: new[] { "TenantId", "TeacherId", "AcademicPeriodId" });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicLoadProposals_TenantId_EducationalProgramId_AcademicPeriodId",
                table: "AcademicLoadProposals",
                columns: new[] { "TenantId", "EducationalProgramId", "AcademicPeriodId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicLoad_AcademicLoadProposals_ProposalId",
                table: "AcademicLoad",
                column: "ProposalId",
                principalTable: "AcademicLoadProposals",
                principalColumn: "ProposalId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcademicLoad_AcademicLoadProposals_ProposalId",
                table: "AcademicLoad");

            migrationBuilder.DropTable(
                name: "AcademicLoadProposals");

            migrationBuilder.DropIndex(
                name: "IX_TeachingSupportHours_ActivityId",
                table: "TeachingSupportHours");

            migrationBuilder.DropIndex(
                name: "IX_TeachingSupportHours_TenantId_AcademicLoadId_ActivityId",
                table: "TeachingSupportHours");

            migrationBuilder.DropIndex(
                name: "IX_AcademicOffering_TenantId_GroupId_SubjectId",
                table: "AcademicOffering");

            migrationBuilder.DropIndex(
                name: "IX_AcademicLoad_ProposalId",
                table: "AcademicLoad");

            migrationBuilder.DropIndex(
                name: "IX_AcademicLoad_TenantId_TeacherId_AcademicPeriodId",
                table: "AcademicLoad");

            migrationBuilder.DropColumn(
                name: "ClassHours",
                table: "AcademicOffering");

            migrationBuilder.DropColumn(
                name: "ProposalId",
                table: "AcademicLoad");

            migrationBuilder.CreateIndex(
                name: "IX_TeachingSupportHours_ActivityId_AcademicLoadId",
                table: "TeachingSupportHours",
                columns: new[] { "ActivityId", "AcademicLoadId" },
                unique: true);
        }
    }
}
