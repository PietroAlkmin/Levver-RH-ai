using LevverRH.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LevverRH.Infra.Data.EntitiesConfiguration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users", "shared");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.AzureAdId)
            .HasMaxLength(100)
            .HasColumnName("azure_ad_id");

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.Nome)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.Role)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(u => u.Ativo)
            .IsRequired();

        builder.Property(u => u.DataCriacao)
            .IsRequired();

        builder.Property(u => u.FotoUrl)
            .HasMaxLength(500);

        builder.Property(u => u.AuthType)
            .IsRequired()
            .HasColumnName("auth_type")
            .HasConversion<int>();

        builder.Property(u => u.PasswordHash)
            .HasMaxLength(255)
            .HasColumnName("password_hash");

        // Relacionamento com Tenant
        builder.HasOne(u => u.Tenant)
            .WithMany()
            .HasForeignKey(u => u.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(u => u.Email);

        builder.HasIndex(u => u.AzureAdId)
            .IsUnique()
            .HasFilter("[azure_ad_id] IS NOT NULL");  // ← CRÍTICO!

        builder.HasIndex(u => u.TenantId);

        // Ignorar Domain Events
        builder.Ignore(u => u.DomainEvents);
    }
}