using LevverRH.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LevverRH.Infra.Data.EntitiesConfiguration;

public class WhiteLabelConfiguration : IEntityTypeConfiguration<WhiteLabel>
{
    public void Configure(EntityTypeBuilder<WhiteLabel> builder)
    {
        builder.ToTable("white_label", "shared");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.LogoUrl)
            .HasMaxLength(500);

        builder.Property(w => w.PrimaryColor)
            .IsRequired()
            .HasMaxLength(7);

        builder.Property(w => w.SecondaryColor)
            .IsRequired()
            .HasMaxLength(7);

        builder.Property(w => w.AccentColor)
            .IsRequired()
            .HasMaxLength(7);

        builder.Property(w => w.BackgroundColor)
            .IsRequired()
            .HasMaxLength(7);

        builder.Property(w => w.TextColor)
            .IsRequired()
            .HasMaxLength(7);

        builder.Property(w => w.BorderColor)
            .IsRequired()
            .HasMaxLength(7);

        builder.Property(w => w.SystemName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(w => w.FaviconUrl)
            .HasMaxLength(500);

        builder.Property(w => w.DominioCustomizado)
            .HasMaxLength(255);

        builder.Property(w => w.EmailRodape)
            .HasMaxLength(500);

        builder.Property(w => w.Active)
            .IsRequired()
            .HasColumnName("active");

        builder.Property(w => w.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(w => w.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        // Relacionamento com Tenant (1:1)
        builder.HasOne(w => w.Tenant)
            .WithOne()
            .HasForeignKey<WhiteLabel>(w => w.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices
        builder.HasIndex(w => w.TenantId).IsUnique();
        builder.HasIndex(w => w.DominioCustomizado).IsUnique();
    }
}