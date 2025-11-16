using LevverRH.Domain.Entities.Talents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LevverRH.Infra.Data.Configurations.Talents
{
    public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
    {
        public void Configure(EntityTypeBuilder<Application> builder)
        {
            builder.ToTable("applications", "TALENTS");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(a => a.DataInscricao)
                .IsRequired();

            builder.Property(a => a.DataAtualizacaoStatus)
                .IsRequired();

            builder.Property(a => a.ScoreGeral)
                .HasColumnType("decimal(5,2)");

            builder.Property(a => a.ScoreTecnico)
                .HasColumnType("decimal(5,2)");

            builder.Property(a => a.ScoreExperiencia)
                .HasColumnType("decimal(5,2)");

            builder.Property(a => a.ScoreCultural)
                .HasColumnType("decimal(5,2)");

            builder.Property(a => a.ScoreSalario)
                .HasColumnType("decimal(5,2)");

            builder.Property(a => a.Favorito)
                .IsRequired()
                .HasDefaultValue(false);

            // Relacionamentos
            builder.HasOne(a => a.Job)
                .WithMany(j => j.Applications)
                .HasForeignKey(a => a.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.Candidate)
                .WithMany(c => c.Applications)
                .HasForeignKey(a => a.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Tenant)
                .WithMany()
                .HasForeignKey(a => a.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Avaliador)
                .WithMany()
                .HasForeignKey(a => a.AvaliadoPor)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices
            builder.HasIndex(a => new { a.JobId, a.Status })
                .HasDatabaseName("idx_talents_applications_job");

            builder.HasIndex(a => a.CandidateId)
                .HasDatabaseName("idx_talents_applications_candidate");

            builder.HasIndex(a => new { a.TenantId, a.Status })
                .HasDatabaseName("idx_talents_applications_tenant_status");

            builder.HasIndex(a => new { a.JobId, a.ScoreGeral })
                .HasDatabaseName("idx_talents_applications_score");
        }
    }
}
