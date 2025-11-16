namespace LevverRH.Domain.Entities.Talents
{
    public class TalentsAuditLog
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid UserId { get; set; }
        public string Acao { get; set; } = string.Empty;
        public string Entidade { get; set; } = string.Empty;
        public Guid EntidadeId { get; set; }
        public string? DetalhesJson { get; set; }
        public string? IpAddress { get; set; }
        public DateTime DataHora { get; set; }

        // Navigation properties
        public Tenant? Tenant { get; set; }
        public User? User { get; set; }
    }
}
