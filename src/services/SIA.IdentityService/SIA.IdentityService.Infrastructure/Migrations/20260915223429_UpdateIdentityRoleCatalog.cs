using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SIA.IdentityService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIdentityRoleCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("98161f96-b782-4283-b5fc-b55851df64d6"),
                columns: new[] { "Code", "Description" },
                values: new object[] { "DivisionHead", "Jefe de división" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "Code", "CreatedAtUtc", "Description", "UpdatedAtUtc" },
                values: new object[,]
                {
                    { new Guid("81d305d5-3af0-4e58-a4e5-2c5b9f4dff18"), "Coordinator", new DateTime(2026, 9, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Coordinador", null },
                    { new Guid("9713d35e-3aee-42bf-920a-97dd1d00c16c"), "Student", new DateTime(2026, 9, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Estudiante", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("81d305d5-3af0-4e58-a4e5-2c5b9f4dff18"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("9713d35e-3aee-42bf-920a-97dd1d00c16c"));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("98161f96-b782-4283-b5fc-b55851df64d6"),
                columns: new[] { "Code", "Description" },
                values: new object[] { "CareerHead", "Jefe de carrera" });
        }
    }
}
