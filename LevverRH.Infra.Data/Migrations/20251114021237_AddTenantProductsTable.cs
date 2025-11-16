using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LevverRH.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantProductsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Adicionar colunas novas em products_catalog
            migrationBuilder.AddColumn<string>(
                name: "icone",
                schema: "shared",
                table: "products_catalog",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cor_primaria",
                schema: "shared",
                table: "products_catalog",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "rota_base",
                schema: "shared",
                table: "products_catalog",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ordem_exibicao",
                schema: "shared",
                table: "products_catalog",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "lancado",
                schema: "shared",
                table: "products_catalog",
                type: "bit",
                nullable: false,
                defaultValue: false);

            // Criar tabela tenant_products
            migrationBuilder.CreateTable(
                name: "tenant_products",
                schema: "shared",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    product_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ativo = table.Column<bool>(type: "bit", nullable: false),
                    data_ativacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    data_desativacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    configuracao_json = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    data_criacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    data_atualizacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tenant_products_products_catalog_product_id",
                        column: x => x.product_id,
                        principalSchema: "shared",
                        principalTable: "products_catalog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tenant_products_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalSchema: "shared",
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // Criar índices
            migrationBuilder.CreateIndex(
                name: "IX_tenant_products_ativo",
                schema: "shared",
                table: "tenant_products",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_products_product_id",
                schema: "shared",
                table: "tenant_products",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_products_tenant_id",
                schema: "shared",
                table: "tenant_products",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_products_tenant_id_product_id",
                schema: "shared",
                table: "tenant_products",
                columns: new[] { "tenant_id", "product_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remover tabela tenant_products
            migrationBuilder.DropTable(
                name: "tenant_products",
                schema: "shared");

            // Remover colunas de products_catalog
            migrationBuilder.DropColumn(
                name: "icone",
                schema: "shared",
                table: "products_catalog");

            migrationBuilder.DropColumn(
                name: "cor_primaria",
                schema: "shared",
                table: "products_catalog");

            migrationBuilder.DropColumn(
                name: "rota_base",
                schema: "shared",
                table: "products_catalog");

            migrationBuilder.DropColumn(
                name: "ordem_exibicao",
                schema: "shared",
                table: "products_catalog");

            migrationBuilder.DropColumn(
                name: "lancado",
                schema: "shared",
                table: "products_catalog");
        }
    }
}
