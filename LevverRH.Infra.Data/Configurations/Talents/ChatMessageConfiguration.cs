using LevverRH.Domain.Entities.Talents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LevverRH.Infra.Data.Configurations.Talents
{
    public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
    {
        public void Configure(EntityTypeBuilder<ChatMessage> builder)
        {
            builder.ToTable("chat_messages", "TALENTS");

            builder.HasKey(cm => cm.Id);

            builder.Property(cm => cm.ConversationId)
                .IsRequired();

            builder.Property(cm => cm.Role)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(cm => cm.Conteudo)
                .IsRequired();

            builder.Property(cm => cm.ModeloUtilizado)
                .HasMaxLength(50);

            builder.Property(cm => cm.TokensUsados)
                .HasDefaultValue(0);

            builder.Property(cm => cm.Timestamp)
                .IsRequired();

            builder.Property(cm => cm.TipoConversa)
                .HasMaxLength(50);

            // Relacionamentos
            builder.HasOne(cm => cm.Tenant)
                .WithMany()
                .HasForeignKey(cm => cm.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cm => cm.User)
                .WithMany()
                .HasForeignKey(cm => cm.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices
            builder.HasIndex(cm => cm.ConversationId)
                .HasDatabaseName("idx_talents_chat_conversation");

            builder.HasIndex(cm => new { cm.TenantId, cm.Timestamp })
                .HasDatabaseName("idx_talents_chat_tenant_data");

            builder.HasIndex(cm => cm.UserId)
                .HasDatabaseName("idx_talents_chat_user");
        }
    }
}
