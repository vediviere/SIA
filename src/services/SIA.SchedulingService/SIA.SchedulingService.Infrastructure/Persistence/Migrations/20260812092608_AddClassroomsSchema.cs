using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIA.SchedulingService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClassroomsSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EventType",
                table: "OutboxMessages",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "ClassroomTypes",
                columns: table => new
                {
                    ClassroomTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassroomTypes", x => x.ClassroomTypeId);
                });

            migrationBuilder.CreateTable(
                name: "ClassroomLabs",
                columns: table => new
                {
                    ClassroomLabId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BuildingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassroomTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassroomLabs", x => x.ClassroomLabId);
                    table.CheckConstraint("CK_Classrooms_Capacity_Positive", "[Capacity] > 0");
                    table.ForeignKey(
                        name: "FK_ClassroomLabs_ClassroomTypes_ClassroomTypeId",
                        column: x => x.ClassroomTypeId,
                        principalTable: "ClassroomTypes",
                        principalColumn: "ClassroomTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_CorrelationId",
                table: "OutboxMessages",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_ProcessedAtUtc",
                table: "OutboxMessages",
                column: "ProcessedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_ClassroomLabs_BuildingId",
                table: "ClassroomLabs",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassroomLabs_ClassroomTypeId",
                table: "ClassroomLabs",
                column: "ClassroomTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassroomLabs_TenantId_Code",
                table: "ClassroomLabs",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassroomTypes_TenantId_Name",
                table: "ClassroomTypes",
                columns: new[] { "TenantId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassroomLabs");

            migrationBuilder.DropTable(
                name: "ClassroomTypes");

            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_CorrelationId",
                table: "OutboxMessages");

            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_ProcessedAtUtc",
                table: "OutboxMessages");

            migrationBuilder.AlterColumn<string>(
                name: "EventType",
                table: "OutboxMessages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);
        }
    }
}
