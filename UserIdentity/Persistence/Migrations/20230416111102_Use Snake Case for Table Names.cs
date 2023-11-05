using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserIdentity.Persistence.Migrations
{
  public partial class UseSnakeCaseforTableNames : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropForeignKey(
          name: "FK_role_claims_roles_RoleId",
          table: "role_claims");

      migrationBuilder.DropForeignKey(
          name: "FK_user_claims_users_UserId",
          table: "user_claims");

      migrationBuilder.DropForeignKey(
          name: "FK_user_logins_users_UserId",
          table: "user_logins");

      migrationBuilder.DropForeignKey(
          name: "FK_user_roles_roles_RoleId",
          table: "user_roles");

      migrationBuilder.DropForeignKey(
          name: "FK_user_roles_users_UserId",
          table: "user_roles");

      migrationBuilder.DropForeignKey(
          name: "FK_user_tokens_users_UserId",
          table: "user_tokens");

      migrationBuilder.DropPrimaryKey(
          name: "PK_users",
          table: "users");

      migrationBuilder.DropPrimaryKey(
          name: "PK_user_tokens",
          table: "user_tokens");

      migrationBuilder.DropPrimaryKey(
          name: "PK_user_roles",
          table: "user_roles");

      migrationBuilder.DropPrimaryKey(
          name: "PK_user_logins",
          table: "user_logins");

      migrationBuilder.DropPrimaryKey(
          name: "PK_user_details",
          table: "user_details");

      migrationBuilder.DropPrimaryKey(
          name: "PK_user_claims",
          table: "user_claims");

      migrationBuilder.DropPrimaryKey(
          name: "PK_roles",
          table: "roles");

      migrationBuilder.DropPrimaryKey(
          name: "PK_role_claims",
          table: "role_claims");

      migrationBuilder.DropPrimaryKey(
          name: "PK_refresh_tokens",
          table: "refresh_tokens");

      migrationBuilder.RenameColumn(
          name: "Email",
          table: "users",
          newName: "email");

      migrationBuilder.RenameColumn(
          name: "Id",
          table: "users",
          newName: "id");

      migrationBuilder.RenameColumn(
          name: "UserName",
          table: "users",
          newName: "user_name");

      migrationBuilder.RenameColumn(
          name: "TwoFactorEnabled",
          table: "users",
          newName: "two_factor_enabled");

      migrationBuilder.RenameColumn(
          name: "SecurityStamp",
          table: "users",
          newName: "security_stamp");

      migrationBuilder.RenameColumn(
          name: "PhoneNumberConfirmed",
          table: "users",
          newName: "phone_number_confirmed");

      migrationBuilder.RenameColumn(
          name: "PhoneNumber",
          table: "users",
          newName: "phone_number");

      migrationBuilder.RenameColumn(
          name: "PasswordHash",
          table: "users",
          newName: "password_hash");

      migrationBuilder.RenameColumn(
          name: "NormalizedUserName",
          table: "users",
          newName: "normalized_user_name");

      migrationBuilder.RenameColumn(
          name: "NormalizedEmail",
          table: "users",
          newName: "normalized_email");

      migrationBuilder.RenameColumn(
          name: "LockoutEnd",
          table: "users",
          newName: "lockout_end");

      migrationBuilder.RenameColumn(
          name: "LockoutEnabled",
          table: "users",
          newName: "lockout_enabled");

      migrationBuilder.RenameColumn(
          name: "EmailConfirmed",
          table: "users",
          newName: "email_confirmed");

      migrationBuilder.RenameColumn(
          name: "ConcurrencyStamp",
          table: "users",
          newName: "concurrency_stamp");

      migrationBuilder.RenameColumn(
          name: "AccessFailedCount",
          table: "users",
          newName: "access_failed_count");

      migrationBuilder.RenameColumn(
          name: "Value",
          table: "user_tokens",
          newName: "value");

      migrationBuilder.RenameColumn(
          name: "Name",
          table: "user_tokens",
          newName: "name");

      migrationBuilder.RenameColumn(
          name: "LoginProvider",
          table: "user_tokens",
          newName: "login_provider");

      migrationBuilder.RenameColumn(
          name: "UserId",
          table: "user_tokens",
          newName: "user_id");

      migrationBuilder.RenameColumn(
          name: "RoleId",
          table: "user_roles",
          newName: "role_id");

      migrationBuilder.RenameColumn(
          name: "UserId",
          table: "user_roles",
          newName: "user_id");

      migrationBuilder.RenameIndex(
          name: "IX_user_roles_RoleId",
          table: "user_roles",
          newName: "ix_user_roles_role_id");

      migrationBuilder.RenameColumn(
          name: "UserId",
          table: "user_logins",
          newName: "user_id");

      migrationBuilder.RenameColumn(
          name: "ProviderDisplayName",
          table: "user_logins",
          newName: "provider_display_name");

      migrationBuilder.RenameColumn(
          name: "ProviderKey",
          table: "user_logins",
          newName: "provider_key");

      migrationBuilder.RenameColumn(
          name: "LoginProvider",
          table: "user_logins",
          newName: "login_provider");

      migrationBuilder.RenameIndex(
          name: "IX_user_logins_UserId",
          table: "user_logins",
          newName: "ix_user_logins_user_id");

      migrationBuilder.RenameColumn(
          name: "Id",
          table: "user_details",
          newName: "id");

      migrationBuilder.RenameColumn(
          name: "UpdatedBy",
          table: "user_details",
          newName: "updated_by");

      migrationBuilder.RenameColumn(
          name: "UpdatedAt",
          table: "user_details",
          newName: "updated_at");

      migrationBuilder.RenameColumn(
          name: "LastName",
          table: "user_details",
          newName: "last_name");

      migrationBuilder.RenameColumn(
          name: "IsDeleted",
          table: "user_details",
          newName: "is_deleted");

      migrationBuilder.RenameColumn(
          name: "ForgotPasswordToken",
          table: "user_details",
          newName: "forgot_password_token");

      migrationBuilder.RenameColumn(
          name: "FirstName",
          table: "user_details",
          newName: "first_name");

      migrationBuilder.RenameColumn(
          name: "EmailConfirmationToken",
          table: "user_details",
          newName: "email_confirmation_token");

      migrationBuilder.RenameColumn(
          name: "CreatedBy",
          table: "user_details",
          newName: "created_by");

      migrationBuilder.RenameColumn(
          name: "CreatedAt",
          table: "user_details",
          newName: "created_at");

      migrationBuilder.RenameColumn(
          name: "Id",
          table: "user_claims",
          newName: "id");

      migrationBuilder.RenameColumn(
          name: "UserId",
          table: "user_claims",
          newName: "user_id");

      migrationBuilder.RenameColumn(
          name: "ClaimValue",
          table: "user_claims",
          newName: "claim_value");

      migrationBuilder.RenameColumn(
          name: "ClaimType",
          table: "user_claims",
          newName: "claim_type");

      migrationBuilder.RenameIndex(
          name: "IX_user_claims_UserId",
          table: "user_claims",
          newName: "ix_user_claims_user_id");

      migrationBuilder.RenameColumn(
          name: "Name",
          table: "roles",
          newName: "name");

      migrationBuilder.RenameColumn(
          name: "Id",
          table: "roles",
          newName: "id");

      migrationBuilder.RenameColumn(
          name: "NormalizedName",
          table: "roles",
          newName: "normalized_name");

      migrationBuilder.RenameColumn(
          name: "ConcurrencyStamp",
          table: "roles",
          newName: "concurrency_stamp");

      migrationBuilder.RenameColumn(
          name: "Id",
          table: "role_claims",
          newName: "id");

      migrationBuilder.RenameColumn(
          name: "RoleId",
          table: "role_claims",
          newName: "role_id");

      migrationBuilder.RenameColumn(
          name: "ClaimValue",
          table: "role_claims",
          newName: "claim_value");

      migrationBuilder.RenameColumn(
          name: "ClaimType",
          table: "role_claims",
          newName: "claim_type");

      migrationBuilder.RenameIndex(
          name: "IX_role_claims_RoleId",
          table: "role_claims",
          newName: "ix_role_claims_role_id");

      migrationBuilder.RenameColumn(
          name: "Token",
          table: "refresh_tokens",
          newName: "token");

      migrationBuilder.RenameColumn(
          name: "Expires",
          table: "refresh_tokens",
          newName: "expires");

      migrationBuilder.RenameColumn(
          name: "Id",
          table: "refresh_tokens",
          newName: "id");

      migrationBuilder.RenameColumn(
          name: "UserId",
          table: "refresh_tokens",
          newName: "user_id");

      migrationBuilder.RenameColumn(
          name: "UpdatedBy",
          table: "refresh_tokens",
          newName: "updated_by");

      migrationBuilder.RenameColumn(
          name: "UpdatedAt",
          table: "refresh_tokens",
          newName: "updated_at");

      migrationBuilder.RenameColumn(
          name: "RemoteIpAddress",
          table: "refresh_tokens",
          newName: "remote_ip_address");

      migrationBuilder.RenameColumn(
          name: "IsDeleted",
          table: "refresh_tokens",
          newName: "is_deleted");

      migrationBuilder.RenameColumn(
          name: "CreatedBy",
          table: "refresh_tokens",
          newName: "created_by");

      migrationBuilder.RenameColumn(
          name: "CreatedAt",
          table: "refresh_tokens",
          newName: "created_at");

      migrationBuilder.RenameIndex(
          name: "IX_refresh_tokens_Token",
          table: "refresh_tokens",
          newName: "ix_refresh_tokens_token");

      migrationBuilder.RenameIndex(
          name: "IX_refresh_tokens_UserId",
          table: "refresh_tokens",
          newName: "ix_refresh_tokens_user_id");

      migrationBuilder.AddPrimaryKey(
          name: "pk_users",
          table: "users",
          column: "id");

      migrationBuilder.AddPrimaryKey(
          name: "pk_user_tokens",
          table: "user_tokens",
          columns: new[] { "user_id", "login_provider", "name" });

      migrationBuilder.AddPrimaryKey(
          name: "pk_user_roles",
          table: "user_roles",
          columns: new[] { "user_id", "role_id" });

      migrationBuilder.AddPrimaryKey(
          name: "pk_user_logins",
          table: "user_logins",
          columns: new[] { "login_provider", "provider_key" });

      migrationBuilder.AddPrimaryKey(
          name: "pk_user_details",
          table: "user_details",
          column: "id");

      migrationBuilder.AddPrimaryKey(
          name: "pk_user_claims",
          table: "user_claims",
          column: "id");

      migrationBuilder.AddPrimaryKey(
          name: "pk_roles",
          table: "roles",
          column: "id");

      migrationBuilder.AddPrimaryKey(
          name: "pk_role_claims",
          table: "role_claims",
          column: "id");

      migrationBuilder.AddPrimaryKey(
          name: "pk_refresh_tokens",
          table: "refresh_tokens",
          column: "id");

      migrationBuilder.AddForeignKey(
          name: "fk_role_claims_roles_role_id",
          table: "role_claims",
          column: "role_id",
          principalTable: "roles",
          principalColumn: "id",
          onDelete: ReferentialAction.Cascade);

      migrationBuilder.AddForeignKey(
          name: "fk_user_claims_users_user_id",
          table: "user_claims",
          column: "user_id",
          principalTable: "users",
          principalColumn: "id",
          onDelete: ReferentialAction.Cascade);

      migrationBuilder.AddForeignKey(
          name: "fk_user_logins_users_user_id",
          table: "user_logins",
          column: "user_id",
          principalTable: "users",
          principalColumn: "id",
          onDelete: ReferentialAction.Cascade);

      migrationBuilder.AddForeignKey(
          name: "fk_user_roles_roles_role_id",
          table: "user_roles",
          column: "role_id",
          principalTable: "roles",
          principalColumn: "id",
          onDelete: ReferentialAction.Cascade);

      migrationBuilder.AddForeignKey(
          name: "fk_user_roles_users_user_id",
          table: "user_roles",
          column: "user_id",
          principalTable: "users",
          principalColumn: "id",
          onDelete: ReferentialAction.Cascade);

      migrationBuilder.AddForeignKey(
          name: "fk_user_tokens_users_user_id",
          table: "user_tokens",
          column: "user_id",
          principalTable: "users",
          principalColumn: "id",
          onDelete: ReferentialAction.Cascade);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropForeignKey(
          name: "fk_role_claims_roles_role_id",
          table: "role_claims");

      migrationBuilder.DropForeignKey(
          name: "fk_user_claims_users_user_id",
          table: "user_claims");

      migrationBuilder.DropForeignKey(
          name: "fk_user_logins_users_user_id",
          table: "user_logins");

      migrationBuilder.DropForeignKey(
          name: "fk_user_roles_roles_role_id",
          table: "user_roles");

      migrationBuilder.DropForeignKey(
          name: "fk_user_roles_users_user_id",
          table: "user_roles");

      migrationBuilder.DropForeignKey(
          name: "fk_user_tokens_users_user_id",
          table: "user_tokens");

      migrationBuilder.DropPrimaryKey(
          name: "pk_users",
          table: "users");

      migrationBuilder.DropPrimaryKey(
          name: "pk_user_tokens",
          table: "user_tokens");

      migrationBuilder.DropPrimaryKey(
          name: "pk_user_roles",
          table: "user_roles");

      migrationBuilder.DropPrimaryKey(
          name: "pk_user_logins",
          table: "user_logins");

      migrationBuilder.DropPrimaryKey(
          name: "pk_user_details",
          table: "user_details");

      migrationBuilder.DropPrimaryKey(
          name: "pk_user_claims",
          table: "user_claims");

      migrationBuilder.DropPrimaryKey(
          name: "pk_roles",
          table: "roles");

      migrationBuilder.DropPrimaryKey(
          name: "pk_role_claims",
          table: "role_claims");

      migrationBuilder.DropPrimaryKey(
          name: "pk_refresh_tokens",
          table: "refresh_tokens");

      migrationBuilder.RenameColumn(
          name: "email",
          table: "users",
          newName: "Email");

      migrationBuilder.RenameColumn(
          name: "id",
          table: "users",
          newName: "Id");

      migrationBuilder.RenameColumn(
          name: "user_name",
          table: "users",
          newName: "UserName");

      migrationBuilder.RenameColumn(
          name: "two_factor_enabled",
          table: "users",
          newName: "TwoFactorEnabled");

      migrationBuilder.RenameColumn(
          name: "security_stamp",
          table: "users",
          newName: "SecurityStamp");

      migrationBuilder.RenameColumn(
          name: "phone_number_confirmed",
          table: "users",
          newName: "PhoneNumberConfirmed");

      migrationBuilder.RenameColumn(
          name: "phone_number",
          table: "users",
          newName: "PhoneNumber");

      migrationBuilder.RenameColumn(
          name: "password_hash",
          table: "users",
          newName: "PasswordHash");

      migrationBuilder.RenameColumn(
          name: "normalized_user_name",
          table: "users",
          newName: "NormalizedUserName");

      migrationBuilder.RenameColumn(
          name: "normalized_email",
          table: "users",
          newName: "NormalizedEmail");

      migrationBuilder.RenameColumn(
          name: "lockout_end",
          table: "users",
          newName: "LockoutEnd");

      migrationBuilder.RenameColumn(
          name: "lockout_enabled",
          table: "users",
          newName: "LockoutEnabled");

      migrationBuilder.RenameColumn(
          name: "email_confirmed",
          table: "users",
          newName: "EmailConfirmed");

      migrationBuilder.RenameColumn(
          name: "concurrency_stamp",
          table: "users",
          newName: "ConcurrencyStamp");

      migrationBuilder.RenameColumn(
          name: "access_failed_count",
          table: "users",
          newName: "AccessFailedCount");

      migrationBuilder.RenameColumn(
          name: "value",
          table: "user_tokens",
          newName: "Value");

      migrationBuilder.RenameColumn(
          name: "name",
          table: "user_tokens",
          newName: "Name");

      migrationBuilder.RenameColumn(
          name: "login_provider",
          table: "user_tokens",
          newName: "LoginProvider");

      migrationBuilder.RenameColumn(
          name: "user_id",
          table: "user_tokens",
          newName: "UserId");

      migrationBuilder.RenameColumn(
          name: "role_id",
          table: "user_roles",
          newName: "RoleId");

      migrationBuilder.RenameColumn(
          name: "user_id",
          table: "user_roles",
          newName: "UserId");

      migrationBuilder.RenameIndex(
          name: "ix_user_roles_role_id",
          table: "user_roles",
          newName: "IX_user_roles_RoleId");

      migrationBuilder.RenameColumn(
          name: "user_id",
          table: "user_logins",
          newName: "UserId");

      migrationBuilder.RenameColumn(
          name: "provider_display_name",
          table: "user_logins",
          newName: "ProviderDisplayName");

      migrationBuilder.RenameColumn(
          name: "provider_key",
          table: "user_logins",
          newName: "ProviderKey");

      migrationBuilder.RenameColumn(
          name: "login_provider",
          table: "user_logins",
          newName: "LoginProvider");

      migrationBuilder.RenameIndex(
          name: "ix_user_logins_user_id",
          table: "user_logins",
          newName: "IX_user_logins_UserId");

      migrationBuilder.RenameColumn(
          name: "id",
          table: "user_details",
          newName: "Id");

      migrationBuilder.RenameColumn(
          name: "updated_by",
          table: "user_details",
          newName: "UpdatedBy");

      migrationBuilder.RenameColumn(
          name: "updated_at",
          table: "user_details",
          newName: "UpdatedAt");

      migrationBuilder.RenameColumn(
          name: "last_name",
          table: "user_details",
          newName: "LastName");

      migrationBuilder.RenameColumn(
          name: "is_deleted",
          table: "user_details",
          newName: "IsDeleted");

      migrationBuilder.RenameColumn(
          name: "forgot_password_token",
          table: "user_details",
          newName: "ForgotPasswordToken");

      migrationBuilder.RenameColumn(
          name: "first_name",
          table: "user_details",
          newName: "FirstName");

      migrationBuilder.RenameColumn(
          name: "email_confirmation_token",
          table: "user_details",
          newName: "EmailConfirmationToken");

      migrationBuilder.RenameColumn(
          name: "created_by",
          table: "user_details",
          newName: "CreatedBy");

      migrationBuilder.RenameColumn(
          name: "created_at",
          table: "user_details",
          newName: "CreatedAt");

      migrationBuilder.RenameColumn(
          name: "id",
          table: "user_claims",
          newName: "Id");

      migrationBuilder.RenameColumn(
          name: "user_id",
          table: "user_claims",
          newName: "UserId");

      migrationBuilder.RenameColumn(
          name: "claim_value",
          table: "user_claims",
          newName: "ClaimValue");

      migrationBuilder.RenameColumn(
          name: "claim_type",
          table: "user_claims",
          newName: "ClaimType");

      migrationBuilder.RenameIndex(
          name: "ix_user_claims_user_id",
          table: "user_claims",
          newName: "IX_user_claims_UserId");

      migrationBuilder.RenameColumn(
          name: "name",
          table: "roles",
          newName: "Name");

      migrationBuilder.RenameColumn(
          name: "id",
          table: "roles",
          newName: "Id");

      migrationBuilder.RenameColumn(
          name: "normalized_name",
          table: "roles",
          newName: "NormalizedName");

      migrationBuilder.RenameColumn(
          name: "concurrency_stamp",
          table: "roles",
          newName: "ConcurrencyStamp");

      migrationBuilder.RenameColumn(
          name: "id",
          table: "role_claims",
          newName: "Id");

      migrationBuilder.RenameColumn(
          name: "role_id",
          table: "role_claims",
          newName: "RoleId");

      migrationBuilder.RenameColumn(
          name: "claim_value",
          table: "role_claims",
          newName: "ClaimValue");

      migrationBuilder.RenameColumn(
          name: "claim_type",
          table: "role_claims",
          newName: "ClaimType");

      migrationBuilder.RenameIndex(
          name: "ix_role_claims_role_id",
          table: "role_claims",
          newName: "IX_role_claims_RoleId");

      migrationBuilder.RenameColumn(
          name: "token",
          table: "refresh_tokens",
          newName: "Token");

      migrationBuilder.RenameColumn(
          name: "expires",
          table: "refresh_tokens",
          newName: "Expires");

      migrationBuilder.RenameColumn(
          name: "id",
          table: "refresh_tokens",
          newName: "Id");

      migrationBuilder.RenameColumn(
          name: "user_id",
          table: "refresh_tokens",
          newName: "UserId");

      migrationBuilder.RenameColumn(
          name: "updated_by",
          table: "refresh_tokens",
          newName: "UpdatedBy");

      migrationBuilder.RenameColumn(
          name: "updated_at",
          table: "refresh_tokens",
          newName: "UpdatedAt");

      migrationBuilder.RenameColumn(
          name: "remote_ip_address",
          table: "refresh_tokens",
          newName: "RemoteIpAddress");

      migrationBuilder.RenameColumn(
          name: "is_deleted",
          table: "refresh_tokens",
          newName: "IsDeleted");

      migrationBuilder.RenameColumn(
          name: "created_by",
          table: "refresh_tokens",
          newName: "CreatedBy");

      migrationBuilder.RenameColumn(
          name: "created_at",
          table: "refresh_tokens",
          newName: "CreatedAt");

      migrationBuilder.RenameIndex(
          name: "ix_refresh_tokens_token",
          table: "refresh_tokens",
          newName: "IX_refresh_tokens_Token");

      migrationBuilder.RenameIndex(
          name: "ix_refresh_tokens_user_id",
          table: "refresh_tokens",
          newName: "IX_refresh_tokens_UserId");

      migrationBuilder.AddPrimaryKey(
          name: "PK_users",
          table: "users",
          column: "Id");

      migrationBuilder.AddPrimaryKey(
          name: "PK_user_tokens",
          table: "user_tokens",
          columns: new[] { "UserId", "LoginProvider", "Name" });

      migrationBuilder.AddPrimaryKey(
          name: "PK_user_roles",
          table: "user_roles",
          columns: new[] { "UserId", "RoleId" });

      migrationBuilder.AddPrimaryKey(
          name: "PK_user_logins",
          table: "user_logins",
          columns: new[] { "LoginProvider", "ProviderKey" });

      migrationBuilder.AddPrimaryKey(
          name: "PK_user_details",
          table: "user_details",
          column: "Id");

      migrationBuilder.AddPrimaryKey(
          name: "PK_user_claims",
          table: "user_claims",
          column: "Id");

      migrationBuilder.AddPrimaryKey(
          name: "PK_roles",
          table: "roles",
          column: "Id");

      migrationBuilder.AddPrimaryKey(
          name: "PK_role_claims",
          table: "role_claims",
          column: "Id");

      migrationBuilder.AddPrimaryKey(
          name: "PK_refresh_tokens",
          table: "refresh_tokens",
          column: "Id");

      migrationBuilder.AddForeignKey(
          name: "FK_role_claims_roles_RoleId",
          table: "role_claims",
          column: "RoleId",
          principalTable: "roles",
          principalColumn: "Id",
          onDelete: ReferentialAction.Cascade);

      migrationBuilder.AddForeignKey(
          name: "FK_user_claims_users_UserId",
          table: "user_claims",
          column: "UserId",
          principalTable: "users",
          principalColumn: "Id",
          onDelete: ReferentialAction.Cascade);

      migrationBuilder.AddForeignKey(
          name: "FK_user_logins_users_UserId",
          table: "user_logins",
          column: "UserId",
          principalTable: "users",
          principalColumn: "Id",
          onDelete: ReferentialAction.Cascade);

      migrationBuilder.AddForeignKey(
          name: "FK_user_roles_roles_RoleId",
          table: "user_roles",
          column: "RoleId",
          principalTable: "roles",
          principalColumn: "Id",
          onDelete: ReferentialAction.Cascade);

      migrationBuilder.AddForeignKey(
          name: "FK_user_roles_users_UserId",
          table: "user_roles",
          column: "UserId",
          principalTable: "users",
          principalColumn: "Id",
          onDelete: ReferentialAction.Cascade);

      migrationBuilder.AddForeignKey(
          name: "FK_user_tokens_users_UserId",
          table: "user_tokens",
          column: "UserId",
          principalTable: "users",
          principalColumn: "Id",
          onDelete: ReferentialAction.Cascade);
    }
  }
}
