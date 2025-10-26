using LevverRH.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LevverRH.Infra.Data.EntitiesConfiguration;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("tenants", "shared");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Nome)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(t => t.Cnpj)
            .IsRequired()
            .HasMaxLength(18);

        builder.Property(t => t.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(t => t.Telefone)
            .HasMaxLength(20);

        builder.Property(t => t.Endereco)
            .HasMaxLength(500);

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(t => t.DataCriacao)
            .IsRequired();

        builder.Property(t => t.DataAtualizacao)
            .IsRequired();

        // Índices
        builder.HasIndex(t => t.Cnpj).IsUnique();
        builder.HasIndex(t => t.Email).IsUnique();

        // Ignorar Domain Events
        builder.Ignore(t => t.DomainEvents);
    }
}