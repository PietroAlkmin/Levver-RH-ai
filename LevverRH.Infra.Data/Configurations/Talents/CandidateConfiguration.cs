using LevverRH.Domain.Entities.Talents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LevverRH.Infra.Data.Configurations.Talents
{
    public class CandidateConfiguration : IEntityTypeConfiguration<Candidate>
    {
        public void Configure(EntityTypeBuilder<Candidate> builder)
        {
            builder.ToTable("candidates", "TALENTS");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Nome)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(c => c.Telefone)
                .HasMaxLength(20);

            builder.Property(c => c.ExperienciaAnos)
                .HasColumnType("decimal(4,1)");

            builder.Property(c => c.NivelSenioridade)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(c => c.Cidade)
                .HasMaxLength(100);

            builder.Property(c => c.Estado)
                .HasMaxLength(2);

            builder.Property(c => c.DisponibilidadeViagem)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.PretensaoSalarial)
                .HasColumnType("decimal(10,2)");

            builder.Property(c => c.DataCadastro)
                .IsRequired();

            builder.Property(c => c.DataAtualizacao)
                .IsRequired();

            builder.Property(c => c.FonteOrigem)
                .HasMaxLength(50);

            // Relacionamentos
            builder.HasOne(c => c.Tenant)
                .WithMany()
                .HasForeignKey(c => c.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices
            builder.HasIndex(c => c.TenantId)
                .HasDatabaseName("idx_talents_candidates_tenant");

            builder.HasIndex(c => c.Email)
                .HasDatabaseName("idx_talents_candidates_email");

            builder.HasIndex(c => new { c.TenantId, c.NivelSenioridade })
                .HasDatabaseName("idx_talents_candidates_nivel");
        }
    }
}
