using LevverRH.Domain.Enums.Talents;

namespace LevverRH.Application.DTOs.Talents
{
    public class JobDTO
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string? Departamento { get; set; }
        public string? Localizacao { get; set; }
        public string? TipoContrato { get; set; }
        public string? ModeloTrabalho { get; set; }
        public decimal? SalarioMin { get; set; }
        public decimal? SalarioMax { get; set; }
        public string? Beneficios { get; set; }
        public string Status { get; set; } = string.Empty;
        public int NumeroVagas { get; set; }
        public DateTime DataCriacao { get; set; }
        public int TotalCandidaturas { get; set; }
    }

    public class CreateJobDTO
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string? Departamento { get; set; }
        public string? Localizacao { get; set; }
        public ContractType? TipoContrato { get; set; }
        public WorkModel? ModeloTrabalho { get; set; }
        public decimal? SalarioMin { get; set; }
        public decimal? SalarioMax { get; set; }
        public string? Beneficios { get; set; }
        public int NumeroVagas { get; set; } = 1;
    }

    public class UpdateJobDTO
    {
        public string? Titulo { get; set; }
        public string? Descricao { get; set; }
        public string? Departamento { get; set; }
        public string? Localizacao { get; set; }
        public ContractType? TipoContrato { get; set; }
        public WorkModel? ModeloTrabalho { get; set; }
        public decimal? SalarioMin { get; set; }
        public decimal? SalarioMax { get; set; }
        public string? Beneficios { get; set; }
        public JobStatus? Status { get; set; }
    }
}
