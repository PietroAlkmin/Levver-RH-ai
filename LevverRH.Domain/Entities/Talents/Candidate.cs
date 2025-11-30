using LevverRH.Domain.Enums.Talents;

namespace LevverRH.Domain.Entities.Talents
{
    public class Candidate
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Telefone { get; set; }
        public string? LinkedinUrl { get; set; }
        public string? CurriculoArquivoUrl { get; set; }
        public string? CurriculoTextoExtraido { get; set; }
        
        // Dados estruturados extraídos do currículo (JSON)
        public string? FormacaoAcademicaJson { get; set; } // [{ curso, instituicao, anoConclusao }]
        public string? ExperienciasProfissionaisJson { get; set; } // [{ empresa, cargo, periodo, descricao }]
        public string? CertificacoesJson { get; set; } // [{ nome, instituicao, ano }]
        public string? IdiomasJson { get; set; } // [{ idioma, nivel }]
        public string? CompetenciasJson { get; set; } // ["Liderança", "Comunicação"]
        
        public decimal? ExperienciaAnos { get; set; }
        public SeniorityLevel? NivelSenioridade { get; set; }
        public string? HabilidadesJson { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public bool DisponibilidadeViagem { get; set; }
        public decimal? PretensaoSalarial { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public string? FonteOrigem { get; set; }

        // Navigation properties
        public Tenant? Tenant { get; set; }
        public ICollection<Application>? Applications { get; set; }
    }
}
