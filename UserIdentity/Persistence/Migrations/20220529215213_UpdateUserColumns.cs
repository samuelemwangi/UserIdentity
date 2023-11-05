using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserIdentity.Persistence.Migrations
{
  public partial class UpdateUserColumns : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AddColumn<string>(
          name: "UserName",
          table: "user_details",
          type: "varchar(128)",
          maxLength: 128,
          nullable: true)
          .Annotation("MySql:CharSet", "utf8mb4");

      migrationBuilder.CreateIndex(
          name: "IX_user_details_UserName",
          table: "user_details",
          column: "UserName");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropIndex(
          name: "IX_user_details_UserName",
          table: "user_details");

      migrationBuilder.DropColumn(
          name: "UserName",
          table: "user_details");
    }
  }
}
