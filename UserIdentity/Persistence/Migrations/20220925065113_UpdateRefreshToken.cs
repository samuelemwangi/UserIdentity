using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserIdentity.Persistence.Migrations;

public partial class UpdateRefreshToken : Migration
{
  protected override void Up(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.DropPrimaryKey(
            name: "PK_refresh_token",
            table: "refresh_token");

    migrationBuilder.DropColumn(
            name: "UserName",
            table: "user_details");

    migrationBuilder.RenameTable(
            name: "refresh_token",
            newName: "refresh_tokens");

    migrationBuilder.RenameIndex(
            name: "IX_refresh_token_UserId",
            table: "refresh_tokens",
            newName: "IX_refresh_tokens_UserId");

    migrationBuilder.RenameIndex(
            name: "IX_refresh_token_Token",
            table: "refresh_tokens",
            newName: "IX_refresh_tokens_Token");

    migrationBuilder.AddPrimaryKey(
            name: "PK_refresh_tokens",
            table: "refresh_tokens",
            column: "Id");
  }

  protected override void Down(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.DropPrimaryKey(
            name: "PK_refresh_tokens",
            table: "refresh_tokens");

    migrationBuilder.RenameTable(
            name: "refresh_tokens",
            newName: "refresh_token");

    migrationBuilder.RenameIndex(
            name: "IX_refresh_tokens_UserId",
            table: "refresh_token",
            newName: "IX_refresh_token_UserId");

    migrationBuilder.RenameIndex(
            name: "IX_refresh_tokens_Token",
            table: "refresh_token",
            newName: "IX_refresh_token_Token");

    migrationBuilder.AddColumn<string>(
            name: "UserName",
            table: "user_details",
            type: "longtext",
            nullable: true)
            .Annotation("MySql:CharSet", "utf8mb4");

    migrationBuilder.AddPrimaryKey(
            name: "PK_refresh_token",
            table: "refresh_token",
            column: "Id");
  }
}
