using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LevverRH.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "shared");

            migrationBuilder.CreateTable(
                name: "audit_logs",
                schema: "shared",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Acao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Entidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EntidadeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DetalhesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DataHora = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "products_catalog",
                schema: "shared",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    produto_nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Categoria = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModeloCobranca = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ValorBasePadrao = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ConfigJsonPadrao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products_catalog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tenants",
                schema: "shared",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Cnpj = table.Column<string>(type: "nvarchar(18)", maxLength: 18, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Endereco = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "integration_credentials",
                schema: "shared",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    plataforma = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    refresh_token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    expires_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    configuracoes_json = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ativo = table.Column<bool>(type: "bit", nullable: false),
                    data_criacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    data_atualizacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_integration_credentials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_integration_credentials_tenants_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "shared",
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tenant_subscriptions",
                schema: "shared",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductCatalogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ValorBaseContratado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ConfigJsonContratado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFim = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ContratoNumero = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Observacoes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tenant_subscriptions_products_catalog_ProductCatalogId",
                        column: x => x.ProductCatalogId,
                        principalSchema: "shared",
                        principalTable: "products_catalog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tenant_subscriptions_tenants_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "shared",
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "shared",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AzureAdId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UltimoLogin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FotoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_users_tenants_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "shared",
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "white_label",
                schema: "shared",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LogoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PrimaryColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    SecondaryColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    AccentColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    BackgroundColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    TextColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    BorderColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    SystemName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FaviconUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DominioCustomizado = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EmailRodape = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_white_label", x => x.Id);
                    table.ForeignKey(
                        name: "FK_white_label_tenants_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "shared",
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_Acao",
                schema: "shared",
                table: "audit_logs",
                column: "Acao");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_DataHora",
                schema: "shared",
                table: "audit_logs",
                column: "DataHora");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_Entidade_EntidadeId",
                schema: "shared",
                table: "audit_logs",
                columns: new[] { "Entidade", "EntidadeId" });

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_TenantId",
                schema: "shared",
                table: "audit_logs",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_UserId",
                schema: "shared",
                table: "audit_logs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_integration_credentials_expires_at",
                schema: "shared",
                table: "integration_credentials",
                column: "expires_at");

            migrationBuilder.CreateIndex(
                name: "IX_integration_credentials_TenantId_plataforma",
                schema: "shared",
                table: "integration_credentials",
                columns: new[] { "TenantId", "plataforma" });

            migrationBuilder.CreateIndex(
                name: "IX_products_catalog_Ativo",
                schema: "shared",
                table: "products_catalog",
                column: "Ativo");

            migrationBuilder.CreateIndex(
                name: "IX_products_catalog_Categoria",
                schema: "shared",
                table: "products_catalog",
                column: "Categoria");

            migrationBuilder.CreateIndex(
                name: "IX_products_catalog_produto_nome",
                schema: "shared",
                table: "products_catalog",
                column: "produto_nome");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_subscriptions_ProductCatalogId",
                schema: "shared",
                table: "tenant_subscriptions",
                column: "ProductCatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_subscriptions_Status",
                schema: "shared",
                table: "tenant_subscriptions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_subscriptions_TenantId",
                schema: "shared",
                table: "tenant_subscriptions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_subscriptions_TenantId_Status",
                schema: "shared",
                table: "tenant_subscriptions",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_tenants_Cnpj",
                schema: "shared",
                table: "tenants",
                column: "Cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tenants_Email",
                schema: "shared",
                table: "tenants",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_AzureAdId",
                schema: "shared",
                table: "users",
                column: "AzureAdId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_Email",
                schema: "shared",
                table: "users",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_users_TenantId",
                schema: "shared",
                table: "users",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_white_label_DominioCustomizado",
                schema: "shared",
                table: "white_label",
                column: "DominioCustomizado",
                unique: true,
                filter: "[DominioCustomizado] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_white_label_TenantId",
                schema: "shared",
                table: "white_label",
                column: "TenantId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_logs",
                schema: "shared");

            migrationBuilder.DropTable(
                name: "integration_credentials",
                schema: "shared");

            migrationBuilder.DropTable(
                name: "tenant_subscriptions",
                schema: "shared");

            migrationBuilder.DropTable(
                name: "users",
                schema: "shared");

            migrationBuilder.DropTable(
                name: "white_label",
                schema: "shared");

            migrationBuilder.DropTable(
                name: "products_catalog",
                schema: "shared");

            migrationBuilder.DropTable(
                name: "tenants",
                schema: "shared");
        }
    }
}
