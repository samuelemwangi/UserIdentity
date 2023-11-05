using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserIdentity.Persistence.Migrations
{
  public partial class UpdateDBSchema : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AlterDatabase()
          .Annotation("MySql:CharSet", "utf8mb4");

      migrationBuilder.CreateTable(
          name: "refresh_token",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
            Token = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            Expires = table.Column<DateTime>(type: "datetime(6)", nullable: false),
            UserId = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            RemoteIpAddress = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            CreatedBy = table.Column<string>(type: "longtext", nullable: true)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
            LastModifiedBy = table.Column<string>(type: "longtext", nullable: true)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            LastModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
            IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_refresh_token", x => x.Id);
          })
          .Annotation("MySql:CharSet", "utf8mb4");

      migrationBuilder.CreateTable(
          name: "roles",
          columns: table => new
          {
            Id = table.Column<string>(type: "varchar(255)", nullable: false)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            NormalizedName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                  .Annotation("MySql:CharSet", "utf8mb4")
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_roles", x => x.Id);
          })
          .Annotation("MySql:CharSet", "utf8mb4");

      migrationBuilder.CreateTable(
          name: "user_details",
          columns: table => new
          {
            Id = table.Column<string>(type: "varchar(255)", nullable: false)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            FirstName = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            LastName = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            EmailConfirmationToken = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            ForgotPasswordToken = table.Column<string>(type: "longtext", nullable: true)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            CreatedBy = table.Column<string>(type: "longtext", nullable: true)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
            LastModifiedBy = table.Column<string>(type: "longtext", nullable: true)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            LastModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
            IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_user_details", x => x.Id);
          })
          .Annotation("MySql:CharSet", "utf8mb4");

      migrationBuilder.CreateTable(
          name: "users",
          columns: table => new
          {
            Id = table.Column<string>(type: "varchar(255)", nullable: false)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            UserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            NormalizedUserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            NormalizedEmail = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            EmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
            PasswordHash = table.Column<string>(type: "longtext", nullable: true)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            SecurityStamp = table.Column<string>(type: "longtext", nullable: true)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            PhoneNumber = table.Column<string>(type: "longtext", nullable: true)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            PhoneNumberConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
            TwoFactorEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
            LockoutEnd = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
            LockoutEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
            AccessFailedCount = table.Column<int>(type: "int", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_users", x => x.Id);
          })
          .Annotation("MySql:CharSet", "utf8mb4");

      migrationBuilder.CreateTable(
          name: "role_claims",
          columns: table => new
          {
            Id = table.Column<int>(type: "int", nullable: false)
                  .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
            RoleId = table.Column<string>(type: "varchar(255)", nullable: false)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            ClaimType = table.Column<string>(type: "longtext", nullable: true)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                  .Annotation("MySql:CharSet", "utf8mb4")
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_role_claims", x => x.Id);
            table.ForeignKey(
                      name: "FK_role_claims_roles_RoleId",
                      column: x => x.RoleId,
                      principalTable: "roles",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          })
          .Annotation("MySql:CharSet", "utf8mb4");

      migrationBuilder.CreateTable(
          name: "user_claims",
          columns: table => new
          {
            Id = table.Column<int>(type: "int", nullable: false)
                  .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
            UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            ClaimType = table.Column<string>(type: "longtext", nullable: true)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                  .Annotation("MySql:CharSet", "utf8mb4")
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_user_claims", x => x.Id);
            table.ForeignKey(
                      name: "FK_user_claims_users_UserId",
                      column: x => x.UserId,
                      principalTable: "users",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          })
          .Annotation("MySql:CharSet", "utf8mb4");

      migrationBuilder.CreateTable(
          name: "user_logins",
          columns: table => new
          {
            LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            ProviderKey = table.Column<string>(type: "varchar(255)", nullable: false)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            ProviderDisplayName = table.Column<string>(type: "longtext", nullable: true)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                  .Annotation("MySql:CharSet", "utf8mb4")
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_user_logins", x => new { x.LoginProvider, x.ProviderKey });
            table.ForeignKey(
                      name: "FK_user_logins_users_UserId",
                      column: x => x.UserId,
                      principalTable: "users",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          })
          .Annotation("MySql:CharSet", "utf8mb4");

      migrationBuilder.CreateTable(
          name: "user_roles",
          columns: table => new
          {
            UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            RoleId = table.Column<string>(type: "varchar(255)", nullable: false)
                  .Annotation("MySql:CharSet", "utf8mb4")
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_user_roles", x => new { x.UserId, x.RoleId });
            table.ForeignKey(
                      name: "FK_user_roles_roles_RoleId",
                      column: x => x.RoleId,
                      principalTable: "roles",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_user_roles_users_UserId",
                      column: x => x.UserId,
                      principalTable: "users",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          })
          .Annotation("MySql:CharSet", "utf8mb4");

      migrationBuilder.CreateTable(
          name: "user_tokens",
          columns: table => new
          {
            UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            Name = table.Column<string>(type: "varchar(255)", nullable: false)
                  .Annotation("MySql:CharSet", "utf8mb4"),
            Value = table.Column<string>(type: "longtext", nullable: true)
                  .Annotation("MySql:CharSet", "utf8mb4")
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_user_tokens", x => new { x.UserId, x.LoginProvider, x.Name });
            table.ForeignKey(
                      name: "FK_user_tokens_users_UserId",
                      column: x => x.UserId,
                      principalTable: "users",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          })
          .Annotation("MySql:CharSet", "utf8mb4");

      migrationBuilder.CreateIndex(
          name: "IX_refresh_token_Token",
          table: "refresh_token",
          column: "Token");

      migrationBuilder.CreateIndex(
          name: "IX_refresh_token_UserId",
          table: "refresh_token",
          column: "UserId");

      migrationBuilder.CreateIndex(
          name: "IX_role_claims_RoleId",
          table: "role_claims",
          column: "RoleId");

      migrationBuilder.CreateIndex(
          name: "RoleNameIndex",
          table: "roles",
          column: "NormalizedName",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_user_claims_UserId",
          table: "user_claims",
          column: "UserId");

      migrationBuilder.CreateIndex(
          name: "IX_user_logins_UserId",
          table: "user_logins",
          column: "UserId");

      migrationBuilder.CreateIndex(
          name: "IX_user_roles_RoleId",
          table: "user_roles",
          column: "RoleId");

      migrationBuilder.CreateIndex(
          name: "EmailIndex",
          table: "users",
          column: "NormalizedEmail");

      migrationBuilder.CreateIndex(
          name: "UserNameIndex",
          table: "users",
          column: "NormalizedUserName",
          unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "refresh_token");

      migrationBuilder.DropTable(
          name: "role_claims");

      migrationBuilder.DropTable(
          name: "user_claims");

      migrationBuilder.DropTable(
          name: "user_details");

      migrationBuilder.DropTable(
          name: "user_logins");

      migrationBuilder.DropTable(
          name: "user_roles");

      migrationBuilder.DropTable(
          name: "user_tokens");

      migrationBuilder.DropTable(
          name: "roles");

      migrationBuilder.DropTable(
          name: "users");
    }
  }
}
