using LevverRH.Domain.Entities.Talents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LevverRH.Infra.Data.Configurations.Talents
{
    public class JobConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            builder.ToTable("jobs", "TALENTS");

            builder.HasKey(j => j.Id);

            builder.Property(j => j.Titulo)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(j => j.Descricao)
                .IsRequired();

            builder.Property(j => j.Departamento)
                .HasMaxLength(100);

            builder.Property(j => j.Localizacao)
                .HasMaxLength(255);

            builder.Property(j => j.TipoContrato)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(j => j.ModeloTrabalho)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(j => j.SalarioMin)
                .HasColumnType("decimal(10,2)");

            builder.Property(j => j.SalarioMax)
                .HasColumnType("decimal(10,2)");

            builder.Property(j => j.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(j => j.DataCriacao)
                .IsRequired();

            builder.Property(j => j.DataAtualizacao)
                .IsRequired();

            builder.Property(j => j.NumeroVagas)
                .IsRequired()
                .HasDefaultValue(1);

            // Relacionamentos
            builder.HasOne(j => j.Tenant)
                .WithMany()
                .HasForeignKey(j => j.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(j => j.Criador)
                .WithMany()
                .HasForeignKey(j => j.CriadoPor)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices
            builder.HasIndex(j => new { j.TenantId, j.Status })
                .HasDatabaseName("idx_talents_jobs_tenant_status");

            builder.HasIndex(j => j.CriadoPor)
                .HasDatabaseName("idx_talents_jobs_criado_por");

            builder.HasIndex(j => j.DataCriacao)
                .HasDatabaseName("idx_talents_jobs_data_criacao");
        }
    }
}
