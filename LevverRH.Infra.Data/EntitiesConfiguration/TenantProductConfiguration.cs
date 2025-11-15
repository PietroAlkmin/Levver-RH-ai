using LevverRH.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LevverRH.Infra.Data.EntitiesConfiguration;

public class TenantProductConfiguration : IEntityTypeConfiguration<TenantProduct>
{
    public void Configure(EntityTypeBuilder<TenantProduct> builder)
    {
        builder.ToTable("tenant_products", "shared");

        builder.HasKey(tp => tp.Id);

        builder.Property(tp => tp.TenantId)
            .IsRequired()
            .HasColumnName("tenant_id");

        builder.Property(tp => tp.ProductId)
            .IsRequired()
            .HasColumnName("product_id");

        builder.Property(tp => tp.Ativo)
            .IsRequired()
            .HasColumnName("ativo");

        builder.Property(tp => tp.DataAtivacao)
            .IsRequired()
            .HasColumnName("data_ativacao");

        builder.Property(tp => tp.DataDesativacao)
            .HasColumnName("data_desativacao");

        builder.Property(tp => tp.ConfiguracaoJson)
            .HasColumnType("nvarchar(max)")
            .HasColumnName("configuracao_json");

        builder.Property(tp => tp.DataCriacao)
            .IsRequired()
            .HasColumnName("data_criacao");

        builder.Property(tp => tp.DataAtualizacao)
            .IsRequired()
            .HasColumnName("data_atualizacao");

        // Relacionamentos
        builder.HasOne(tp => tp.Tenant)
            .WithMany()
            .HasForeignKey(tp => tp.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(tp => tp.Product)
            .WithMany()
            .HasForeignKey(tp => tp.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(tp => tp.TenantId);
        builder.HasIndex(tp => tp.ProductId);
        builder.HasIndex(tp => new { tp.TenantId, tp.ProductId }).IsUnique();
        builder.HasIndex(tp => tp.Ativo);
    }
}
