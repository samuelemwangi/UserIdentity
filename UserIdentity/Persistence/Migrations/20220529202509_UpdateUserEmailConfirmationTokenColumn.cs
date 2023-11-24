using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserIdentity.Persistence.Migrations
{
	public partial class UpdateUserEmailConfirmationTokenColumn : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<string>(
					name: "EmailConfirmationToken",
					table: "user_details",
					type: "varchar(600)",
					maxLength: 600,
					nullable: true,
					oldClrType: typeof(string),
					oldType: "varchar(200)",
					oldMaxLength: 200,
					oldNullable: true)
					.Annotation("MySql:CharSet", "utf8mb4")
					.OldAnnotation("MySql:CharSet", "utf8mb4");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<string>(
					name: "EmailConfirmationToken",
					table: "user_details",
					type: "varchar(200)",
					maxLength: 200,
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
