using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LevverRH.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTalentsSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "TALENTS");

            migrationBuilder.CreateTable(
                name: "audit_logs",
                schema: "TALENTS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Acao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Entidade = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntidadeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DetalhesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    DataHora = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_audit_logs_tenants_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "shared",
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_audit_logs_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "shared",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "candidates",
                schema: "TALENTS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LinkedinUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurriculoArquivoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurriculoTextoExtraido = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExperienciaAnos = table.Column<decimal>(type: "decimal(4,1)", nullable: true),
                    NivelSenioridade = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    HabilidadesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    DisponibilidadeViagem = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    PretensaoSalarial = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FonteOrigem = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_candidates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_candidates_tenants_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "shared",
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "chat_messages",
                schema: "TALENTS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConversationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoConversa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Conteudo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContextoJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TokensUsados = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    ModeloUtilizado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chat_messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_chat_messages_tenants_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "shared",
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_chat_messages_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "shared",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "jobs",
                schema: "TALENTS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Departamento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Localizacao = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TipoContrato = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModeloTrabalho = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SalarioMin = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    SalarioMax = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Beneficios = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequirementsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CriadoPor = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFechamento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PublicationsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumeroVagas = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_jobs_tenants_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "shared",
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_jobs_users_CriadoPor",
                        column: x => x.CriadoPor,
                        principalSchema: "shared",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "usage_metrics",
                schema: "TALENTS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Periodo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VagasAbertas = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    VagasNaFranquia = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    VagasExtras = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ValorBase = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    ValorExtra = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    ValorTotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    DetalhesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usage_metrics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_usage_metrics_tenants_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "shared",
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "applications",
                schema: "TALENTS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CandidateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DataInscricao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacaoStatus = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScoreGeral = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    ScoreTecnico = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    ScoreExperiencia = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    ScoreCultural = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    ScoreSalario = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    JustificativaIA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PontosFortes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PontosAtencao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecomendacaoIA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataCalculoScore = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AvaliadoPor = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NotasAvaliador = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Favorito = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_applications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_applications_candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalSchema: "TALENTS",
                        principalTable: "candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_applications_jobs_JobId",
                        column: x => x.JobId,
                        principalSchema: "TALENTS",
                        principalTable: "jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_applications_tenants_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "shared",
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_applications_users_AvaliadoPor",
                        column: x => x.AvaliadoPor,
                        principalSchema: "shared",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "idx_talents_applications_candidate",
                schema: "TALENTS",
                table: "applications",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "idx_talents_applications_job",
                schema: "TALENTS",
                table: "applications",
                columns: new[] { "JobId", "Status" });

            migrationBuilder.CreateIndex(
                name: "idx_talents_applications_score",
                schema: "TALENTS",
                table: "applications",
                columns: new[] { "JobId", "ScoreGeral" });

            migrationBuilder.CreateIndex(
                name: "idx_talents_applications_tenant_status",
                schema: "TALENTS",
                table: "applications",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_applications_AvaliadoPor",
                schema: "TALENTS",
                table: "applications",
                column: "AvaliadoPor");

            migrationBuilder.CreateIndex(
                name: "idx_talents_audit_entidade",
                schema: "TALENTS",
                table: "audit_logs",
                columns: new[] { "Entidade", "EntidadeId" });

            migrationBuilder.CreateIndex(
                name: "idx_talents_audit_tenant_data",
                schema: "TALENTS",
                table: "audit_logs",
                columns: new[] { "TenantId", "DataHora" });

            migrationBuilder.CreateIndex(
                name: "idx_talents_audit_user",
                schema: "TALENTS",
                table: "audit_logs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "idx_talents_candidates_email",
                schema: "TALENTS",
                table: "candidates",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "idx_talents_candidates_nivel",
                schema: "TALENTS",
                table: "candidates",
                columns: new[] { "TenantId", "NivelSenioridade" });

            migrationBuilder.CreateIndex(
                name: "idx_talents_candidates_tenant",
                schema: "TALENTS",
                table: "candidates",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "idx_talents_chat_conversation",
                schema: "TALENTS",
                table: "chat_messages",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "idx_talents_chat_tenant_data",
                schema: "TALENTS",
                table: "chat_messages",
                columns: new[] { "TenantId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "idx_talents_chat_user",
                schema: "TALENTS",
                table: "chat_messages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "idx_talents_jobs_criado_por",
                schema: "TALENTS",
                table: "jobs",
                column: "CriadoPor");

            migrationBuilder.CreateIndex(
                name: "idx_talents_jobs_data_criacao",
                schema: "TALENTS",
                table: "jobs",
                column: "DataCriacao");

            migrationBuilder.CreateIndex(
                name: "idx_talents_jobs_tenant_status",
                schema: "TALENTS",
                table: "jobs",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "idx_talents_metrics_periodo",
                schema: "TALENTS",
                table: "usage_metrics",
                column: "Periodo");

            migrationBuilder.CreateIndex(
                name: "idx_talents_metrics_unique",
                schema: "TALENTS",
                table: "usage_metrics",
                columns: new[] { "TenantId", "Periodo" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "applications",
                schema: "TALENTS");

            migrationBuilder.DropTable(
                name: "audit_logs",
                schema: "TALENTS");

            migrationBuilder.DropTable(
                name: "chat_messages",
                schema: "TALENTS");

            migrationBuilder.DropTable(
                name: "usage_metrics",
                schema: "TALENTS");

            migrationBuilder.DropTable(
                name: "candidates",
                schema: "TALENTS");

            migrationBuilder.DropTable(
                name: "jobs",
                schema: "TALENTS");
        }
    }
}
