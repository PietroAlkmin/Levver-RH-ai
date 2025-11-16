namespace LevverRH.Domain.Entities.Talents
{
    public class ChatMessage
    {
        public Guid Id { get; set; }
        public Guid ConversationId { get; set; }
        public Guid UserId { get; set; }
        public Guid TenantId { get; set; }
        public string TipoConversa { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // user, assistant
        public string Conteudo { get; set; } = string.Empty;
        public string? ContextoJson { get; set; }
        public DateTime Timestamp { get; set; }
        public int? TokensUsados { get; set; }
        public string? ModeloUtilizado { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public Tenant? Tenant { get; set; }
    }
}
