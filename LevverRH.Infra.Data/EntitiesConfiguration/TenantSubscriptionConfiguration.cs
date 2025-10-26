using LevverRH.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LevverRH.Infra.Data.EntitiesConfiguration;

public class TenantSubscriptionConfiguration : IEntityTypeConfiguration<TenantSubscription>
{
    public void Configure(EntityTypeBuilder<TenantSubscription> builder)
    {
        builder.ToTable("tenant_subscriptions", "shared");

        builder.HasKey(ts => ts.Id);

        builder.Property(ts => ts.ValorBaseContratado)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(ts => ts.ConfigJsonContratado)
            .HasColumnType("nvarchar(max)");

        builder.Property(ts => ts.DataInicio)
            .IsRequired();

        builder.Property(ts => ts.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(ts => ts.ContratoNumero)
            .HasMaxLength(100);

        builder.Property(ts => ts.Observacoes)
            .HasMaxLength(1000);

        builder.Property(ts => ts.DataCriacao)
            .IsRequired();

        builder.Property(ts => ts.DataAtualizacao)
            .IsRequired();

        // Relacionamento com Tenant
        builder.HasOne(ts => ts.Tenant)
            .WithMany()
            .HasForeignKey(ts => ts.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relacionamento com ProductCatalog
        builder.HasOne(ts => ts.ProductCatalog)
            .WithMany()
            .HasForeignKey(ts => ts.ProductCatalogId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(ts => ts.TenantId);
        builder.HasIndex(ts => ts.ProductCatalogId);
        builder.HasIndex(ts => ts.Status);
        builder.HasIndex(ts => new { ts.TenantId, ts.Status });

        // Ignorar Domain Events
        builder.Ignore(ts => ts.DomainEvents);
    }
}