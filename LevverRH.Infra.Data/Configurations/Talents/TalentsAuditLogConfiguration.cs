using LevverRH.Domain.Entities.Talents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LevverRH.Infra.Data.Configurations.Talents
{
    public class TalentsAuditLogConfiguration : IEntityTypeConfiguration<TalentsAuditLog>
    {
        public void Configure(EntityTypeBuilder<TalentsAuditLog> builder)
        {
            builder.ToTable("audit_logs", "TALENTS");

            builder.HasKey(al => al.Id);

            builder.Property(al => al.Acao)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(al => al.Entidade)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(al => al.DataHora)
                .IsRequired();

            builder.Property(al => al.IpAddress)
                .HasMaxLength(45);

            // Relacionamentos
            builder.HasOne(al => al.Tenant)
                .WithMany()
                .HasForeignKey(al => al.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(al => al.User)
                .WithMany()
                .HasForeignKey(al => al.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices
            builder.HasIndex(al => new { al.TenantId, al.DataHora })
                .HasDatabaseName("idx_talents_audit_tenant_data");

            builder.HasIndex(al => new { al.Entidade, al.EntidadeId })
                .HasDatabaseName("idx_talents_audit_entidade");

            builder.HasIndex(al => al.UserId)
                .HasDatabaseName("idx_talents_audit_user");
        }
    }
}
