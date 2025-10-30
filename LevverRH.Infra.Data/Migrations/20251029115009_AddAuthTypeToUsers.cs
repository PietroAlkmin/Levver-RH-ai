using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LevverRH.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthTypeToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_AzureAdId",
                schema: "shared",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "AzureAdId",
                schema: "shared",
                table: "users",
                newName: "azure_ad_id");

            migrationBuilder.AlterColumn<string>(
                name: "azure_ad_id",
                schema: "shared",
                table: "users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "auth_type",
                schema: "shared",
                table: "users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "password_hash",
                schema: "shared",
                table: "users",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_azure_ad_id",
                schema: "shared",
                table: "users",
                column: "azure_ad_id",
                unique: true,
                filter: "[azure_ad_id] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_azure_ad_id",
                schema: "shared",
                table: "users");

            migrationBuilder.DropColumn(
                name: "auth_type",
                schema: "shared",
                table: "users");

            migrationBuilder.DropColumn(
                name: "password_hash",
                schema: "shared",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "azure_ad_id",
                schema: "shared",
                table: "users",
                newName: "AzureAdId");

            migrationBuilder.AlterColumn<string>(
                name: "AzureAdId",
                schema: "shared",
                table: "users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_AzureAdId",
                schema: "shared",
                table: "users",
                column: "AzureAdId",
                unique: true);
        }
    }
}
