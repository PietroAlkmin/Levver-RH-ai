namespace LevverRH.Domain.Entities.Talents
{
    public class UsageMetric
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public DateTime Periodo { get; set; }
        public int VagasAbertas { get; set; }
        public int VagasNaFranquia { get; set; }
        public int VagasExtras { get; set; }
        public decimal ValorBase { get; set; }
        public decimal ValorExtra { get; set; }
        public decimal ValorTotal { get; set; }
        public string? DetalhesJson { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Tenant? Tenant { get; set; }
    }
}
