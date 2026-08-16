using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIA.SchedulingService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ClassSchedules_OfferingId",
                table: "ClassSchedules",
                column: "OfferingId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassSchedules_AcademicOffering_OfferingId",
                table: "ClassSchedules",
                column: "OfferingId",
                principalTable: "AcademicOffering",
                principalColumn: "OfferingId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupportSchedules_TeachingSupportHours_SupportHourId",
                table: "SupportSchedules",
                column: "SupportHourId",
                principalTable: "TeachingSupportHours",
                principalColumn: "SupportHourId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassSchedules_AcademicOffering_OfferingId",
                table: "ClassSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_SupportSchedules_TeachingSupportHours_SupportHourId",
                table: "SupportSchedules");

            migrationBuilder.DropIndex(
                name: "IX_ClassSchedules_OfferingId",
                table: "ClassSchedules");
        }
    }
}
