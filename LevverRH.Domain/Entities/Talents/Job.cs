using LevverRH.Domain.Enums.Talents;

namespace LevverRH.Domain.Entities.Talents
{
    public class Job
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string? Departamento { get; set; }
        public string? Localizacao { get; set; }
        public ContractType? TipoContrato { get; set; }
        public WorkModel? ModeloTrabalho { get; set; }
        public decimal? SalarioMin { get; set; }
        public decimal? SalarioMax { get; set; }
        public string? Beneficios { get; set; }
        public string? RequirementsJson { get; set; }
        public JobStatus Status { get; set; }
        public Guid CriadoPor { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public DateTime? DataFechamento { get; set; }
        public string? PublicationsJson { get; set; }
        public int NumeroVagas { get; set; }

        // Navigation properties
        public Tenant? Tenant { get; set; }
        public User? Criador { get; set; }
        public ICollection<Application>? Applications { get; set; }
    }
}
