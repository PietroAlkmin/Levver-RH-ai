using LevverRH.Domain.Enums.Talents;

namespace LevverRH.Application.DTOs.Talents
{
    public class CandidateDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Telefone { get; set; }
        public string? LinkedinUrl { get; set; }
        public decimal? ExperienciaAnos { get; set; }
        public string? NivelSenioridade { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public decimal? PretensaoSalarial { get; set; }
        public DateTime DataCadastro { get; set; }
    }

    public class CreateCandidateDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Telefone { get; set; }
        public string? LinkedinUrl { get; set; }
        public decimal? ExperienciaAnos { get; set; }
        public SeniorityLevel? NivelSenioridade { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public bool DisponibilidadeViagem { get; set; }
        public decimal? PretensaoSalarial { get; set; }
    }
}
