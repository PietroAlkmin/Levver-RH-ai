using LevverRH.Domain.Entities.Talents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LevverRH.Infra.Data.Configurations.Talents;

public class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.ToTable("jobs", "TALENTS");

        builder.HasKey(j => j.Id);

        // ========== INFORMAÇÕES BÁSICAS ==========
        builder.Property(j => j.Titulo)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(j => j.Descricao)
            .IsRequired();

        builder.Property(j => j.Departamento)
            .HasMaxLength(100);

        builder.Property(j => j.NumeroVagas)
            .IsRequired()
            .HasDefaultValue(1);

        // ========== LOCALIZAÇÃO E FORMATO ==========
        builder.Property(j => j.Localizacao)
            .HasMaxLength(255);

        builder.Property(j => j.Cidade)
            .HasMaxLength(100);

        builder.Property(j => j.Estado)
            .HasMaxLength(2);

        builder.Property(j => j.TipoContrato)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(j => j.ModeloTrabalho)
            .HasConversion<string>()
            .HasMaxLength(50);

        // ========== REQUISITOS ==========
        builder.Property(j => j.AnosExperienciaMinimo);

        builder.Property(j => j.FormacaoNecessaria)
            .HasMaxLength(255);

        builder.Property(j => j.ConhecimentosObrigatorios)
            .HasColumnType("NVARCHAR(MAX)");

        builder.Property(j => j.ConhecimentosDesejaveis)
            .HasColumnType("NVARCHAR(MAX)");

        builder.Property(j => j.CompetenciasImportantes)
            .HasColumnType("NVARCHAR(MAX)");

        // ========== RESPONSABILIDADES ==========
        builder.Property(j => j.Responsabilidades)
            .HasColumnType("NVARCHAR(MAX)");

        // ========== REMUNERAÇÃO ==========
        builder.Property(j => j.SalarioMin)
            .HasColumnType("decimal(10,2)");

        builder.Property(j => j.SalarioMax)
            .HasColumnType("decimal(10,2)");

        builder.Property(j => j.Beneficios)
            .HasColumnType("NVARCHAR(MAX)");

        builder.Property(j => j.BonusComissao)
            .HasMaxLength(500);

        // ========== PROCESSO SELETIVO ==========
        builder.Property(j => j.EtapasProcesso)
            .HasColumnType("NVARCHAR(MAX)");

        builder.Property(j => j.TiposTesteEntrevista)
            .HasColumnType("NVARCHAR(MAX)");

        builder.Property(j => j.PrevisaoInicio);

        // ========== SOBRE A VAGA ==========
        builder.Property(j => j.SobreTime)
            .HasColumnType("NVARCHAR(MAX)");

        builder.Property(j => j.Diferenciais)
            .HasColumnType("NVARCHAR(MAX)");

        // ========== CONTROLE IA ==========
        builder.Property(j => j.ConversationId);

        builder.Property(j => j.IaCompletionPercentage)
            .HasColumnType("decimal(5,2)");

        // ========== STATUS E AUDITORIA ==========
        builder.Property(j => j.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(j => j.DataCriacao)
            .IsRequired();

        builder.Property(j => j.DataAtualizacao)
            .IsRequired();

        builder.Property(j => j.DataFechamento);

        // ========== RELACIONAMENTOS ==========
        builder.HasOne(j => j.Tenant)
            .WithMany()
            .HasForeignKey(j => j.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(j => j.Criador)
            .WithMany()
            .HasForeignKey(j => j.CriadoPor)
            .OnDelete(DeleteBehavior.Restrict);

        // ========== ÍNDICES ==========
        builder.HasIndex(j => new { j.TenantId, j.Status })
            .HasDatabaseName("idx_talents_jobs_tenant_status");

        builder.HasIndex(j => j.CriadoPor)
            .HasDatabaseName("idx_talents_jobs_criado_por");

        builder.HasIndex(j => j.DataCriacao)
            .HasDatabaseName("idx_talents_jobs_data_criacao");

        builder.HasIndex(j => j.ConversationId)
            .HasDatabaseName("idx_talents_jobs_conversation_id");
    }
}
