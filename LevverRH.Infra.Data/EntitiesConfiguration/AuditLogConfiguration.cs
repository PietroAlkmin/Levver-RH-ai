using LevverRH.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LevverRH.Infra.Data.EntitiesConfiguration;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs", "shared");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Acao)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Entidade)
            .HasMaxLength(100);

        builder.Property(a => a.DetalhesJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(a => a.IpAddress)
            .HasMaxLength(50);

        builder.Property(a => a.UserAgent)
            .HasMaxLength(500);

        builder.Property(a => a.DataHora)
            .IsRequired();

        // Índices para consultas rápidas
        builder.HasIndex(a => a.TenantId);
        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => a.Acao);
        builder.HasIndex(a => a.DataHora);
        builder.HasIndex(a => new { a.Entidade, a.EntidadeId });
    }
}