using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIA.SchoolControlService.Infrastructure.Persistence.Migrations
{
  public partial class SplitLastNames : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.RenameColumn(
          name: "LastNames",
          table: "Students",
          newName: "PaternalLastName");

      migrationBuilder.AlterColumn<string>(
          name: "PaternalLastName",
          table: "Students",
          type: "nvarchar(100)",
          maxLength: 100,
          nullable: false,
          oldClrType: typeof(string),
          oldType: "nvarchar(200)",
          oldMaxLength: 200);

      migrationBuilder.AddColumn<string>(
          name: "MaternalLastName",
          table: "Students",
          type: "nvarchar(100)",
          maxLength: 100,
          nullable: true);

      migrationBuilder.Sql(
          "UPDATE [Students] " +
          "SET [MaternalLastName] = N'' " +
          "WHERE [MaternalLastName] IS NULL;");

      migrationBuilder.AlterColumn<string>(
          name: "MaternalLastName",
          table: "Students",
          type: "nvarchar(100)",
          maxLength: 100,
          nullable: false,
          oldClrType: typeof(string),
          oldType: "nvarchar(100)",
          oldMaxLength: 100,
          oldNullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropColumn(
          name: "MaternalLastName",
          table: "Students");

      migrationBuilder.AlterColumn<string>(
          name: "PaternalLastName",
          table: "Students",
          type: "nvarchar(200)",
          maxLength: 200,
          nullable: false,
          oldClrType: typeof(string),
          oldType: "nvarchar(100)",
          oldMaxLength: 100);

      migrationBuilder.RenameColumn(
          name: "PaternalLastName",
          table: "Students",
          newName: "LastNames");
    }
  }
}
