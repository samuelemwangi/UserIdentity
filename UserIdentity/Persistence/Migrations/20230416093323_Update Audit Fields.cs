using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserIdentity.Persistence.Migrations
{
  public partial class UpdateAuditFields : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.RenameColumn(
          name: "LastModifiedDate",
          table: "user_details",
          newName: "UpdatedAt");

      migrationBuilder.RenameColumn(
          name: "LastModifiedBy",
          table: "user_details",
          newName: "UpdatedBy");

      migrationBuilder.RenameColumn(
          name: "CreatedDate",
          table: "user_details",
          newName: "CreatedAt");

      migrationBuilder.RenameColumn(
          name: "LastModifiedDate",
          table: "refresh_tokens",
          newName: "UpdatedAt");

      migrationBuilder.RenameColumn(
          name: "LastModifiedBy",
          table: "refresh_tokens",
          newName: "UpdatedBy");

      migrationBuilder.RenameColumn(
          name: "CreatedDate",
          table: "refresh_tokens",
          newName: "CreatedAt");

      migrationBuilder.AlterColumn<string>(
          name: "ForgotPasswordToken",
          table: "user_details",
          type: "varchar(600)",
          maxLength: 600,
          nullable: true,
          oldClrType: typeof(string),
          oldType: "longtext",
          oldNullable: true)
          .Annotation("MySql:CharSet", "utf8mb4")
          .OldAnnotation("MySql:CharSet", "utf8mb4");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.RenameColumn(
          name: "UpdatedBy",
          table: "user_details",
          newName: "LastModifiedBy");

      migrationBuilder.RenameColumn(
          name: "UpdatedAt",
          table: "user_details",
          newName: "LastModifiedDate");

      migrationBuilder.RenameColumn(
          name: "CreatedAt",
          table: "user_details",
          newName: "CreatedDate");

      migrationBuilder.RenameColumn(
          name: "UpdatedBy",
          table: "refresh_tokens",
          newName: "LastModifiedBy");

      migrationBuilder.RenameColumn(
          name: "UpdatedAt",
          table: "refresh_tokens",
          newName: "LastModifiedDate");

      migrationBuilder.RenameColumn(
          name: "CreatedAt",
          table: "refresh_tokens",
          newName: "CreatedDate");

      migrationBuilder.AlterColumn<string>(
          name: "ForgotPasswordToken",
          table: "user_details",
          type: "longtext",
          nullable: true,
          oldClrType: typeof(string),
          oldType: "varchar(600)",
          oldMaxLength: 600,
          oldNullable: true)
          .Annotation("MySql:CharSet", "utf8mb4")
          .OldAnnotation("MySql:CharSet", "utf8mb4");
    }
  }
}
