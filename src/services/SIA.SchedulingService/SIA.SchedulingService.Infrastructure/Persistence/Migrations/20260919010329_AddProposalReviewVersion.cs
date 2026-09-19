using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIA.SchedulingService.Infrastructure.Persistence.Migrations
{
  /// <inheritdoc />
  public partial class AddProposalReviewVersion : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AddColumn<int>(
          name: "ReviewVersion",
          table: "AcademicLoadProposals",
          type: "int",
          nullable: false,
          defaultValue: 0);

      migrationBuilder.Sql(
          """
        UPDATE [AcademicLoadProposals]
        SET [ReviewVersion] = 1
        WHERE [ProposalStatus] IN (2, 3, 4);
        """);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropColumn(
          name: "ReviewVersion",
          table: "AcademicLoadProposals");
    }
  }
}
