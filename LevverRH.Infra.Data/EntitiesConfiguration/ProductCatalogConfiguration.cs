using LevverRH.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LevverRH.Infra.Data.EntitiesConfiguration;

public class ProductCatalogConfiguration : IEntityTypeConfiguration<ProductCatalog>
{
    public void Configure(EntityTypeBuilder<ProductCatalog> builder)
    {
        builder.ToTable("products_catalog", "shared");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.ProdutoNome)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("produto_nome");

        builder.Property(p => p.Descricao)
            .HasMaxLength(1000);

        builder.Property(p => p.Categoria)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Icone)
            .HasMaxLength(200)
            .HasColumnName("icone");

        builder.Property(p => p.CorPrimaria)
            .HasMaxLength(20)
            .HasColumnName("cor_primaria");

        builder.Property(p => p.RotaBase)
            .HasMaxLength(100)
            .HasColumnName("rota_base");

        builder.Property(p => p.OrdemExibicao)
            .IsRequired()
            .HasColumnName("ordem_exibicao");

        builder.Property(p => p.Lancado)
            .IsRequired()
            .HasColumnName("lancado");

        builder.Property(p => p.ModeloCobranca)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(p => p.ValorBasePadrao)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.ConfigJsonPadrao)
            .HasColumnType("nvarchar(max)");

        builder.Property(p => p.Ativo)
            .IsRequired();

        builder.Property(p => p.DataCriacao)
            .IsRequired();

        builder.Property(p => p.DataAtualizacao)
            .IsRequired();

        // Índices
        builder.HasIndex(p => p.ProdutoNome);
        builder.HasIndex(p => p.Categoria);
        builder.HasIndex(p => p.Ativo);
    }
}