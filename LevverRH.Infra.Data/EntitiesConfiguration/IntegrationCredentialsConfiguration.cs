using LevverRH.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LevverRH.Infra.Data.EntitiesConfiguration;

public class IntegrationCredentialsConfiguration : IEntityTypeConfiguration<IntegrationCredentials>
{
    public void Configure(EntityTypeBuilder<IntegrationCredentials> builder)
    {
     builder.ToTable("integration_credentials", "shared");

   builder.HasKey(i => i.Id);

  builder.Property(i => i.Plataforma)
       .IsRequired()
  .HasMaxLength(50)
   .HasColumnName("plataforma");

     builder.Property(i => i.Token)
    .IsRequired()
        .HasColumnType("nvarchar(max)")
     .HasColumnName("token");

        builder.Property(i => i.RefreshToken)
  .HasColumnType("nvarchar(max)")
      .HasColumnName("refresh_token");

        builder.Property(i => i.ExpiresAt)
     .HasColumnName("expires_at");

  builder.Property(i => i.ConfiguracoesJson)
     .HasColumnType("nvarchar(max)")
            .HasColumnName("configuracoes_json");

  builder.Property(i => i.Ativo)
   .IsRequired()
   .HasColumnName("ativo");

        builder.Property(i => i.DataCriacao)
  .IsRequired()
   .HasColumnName("data_criacao");

        builder.Property(i => i.DataAtualizacao)
 .IsRequired()
   .HasColumnName("data_atualizacao");

 // Relacionamento com Tenant
        builder.HasOne(i => i.Tenant)
 .WithMany()
    .HasForeignKey(i => i.TenantId)
      .OnDelete(DeleteBehavior.Cascade);

        // Índices
        builder.HasIndex(i => new { i.TenantId, i.Plataforma });
        builder.HasIndex(i => i.ExpiresAt);
    }
}
