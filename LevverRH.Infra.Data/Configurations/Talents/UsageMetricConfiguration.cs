using LevverRH.Domain.Entities.Talents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LevverRH.Infra.Data.Configurations.Talents
{
    public class UsageMetricConfiguration : IEntityTypeConfiguration<UsageMetric>
    {
        public void Configure(EntityTypeBuilder<UsageMetric> builder)
        {
            builder.ToTable("usage_metrics", "TALENTS");

            builder.HasKey(um => um.Id);

            builder.Property(um => um.Periodo)
                .IsRequired();

            builder.Property(um => um.VagasAbertas)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(um => um.VagasNaFranquia)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(um => um.VagasExtras)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(um => um.ValorBase)
                .HasColumnType("decimal(10,2)")
                .HasDefaultValue(0);

            builder.Property(um => um.ValorExtra)
                .HasColumnType("decimal(10,2)")
                .HasDefaultValue(0);

            builder.Property(um => um.ValorTotal)
                .HasColumnType("decimal(10,2)")
                .HasDefaultValue(0);

            builder.Property(um => um.CreatedAt)
                .IsRequired();

            // Relacionamentos
            builder.HasOne(um => um.Tenant)
                .WithMany()
                .HasForeignKey(um => um.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices
            builder.HasIndex(um => new { um.TenantId, um.Periodo })
                .IsUnique()
                .HasDatabaseName("idx_talents_metrics_unique");

            builder.HasIndex(um => um.Periodo)
                .HasDatabaseName("idx_talents_metrics_periodo");
        }
    }
}
