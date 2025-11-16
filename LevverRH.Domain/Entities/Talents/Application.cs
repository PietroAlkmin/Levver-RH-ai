using LevverRH.Domain.Enums.Talents;

namespace LevverRH.Domain.Entities.Talents
{
    public class Application
    {
        public Guid Id { get; set; }
        public Guid JobId { get; set; }
        public Guid CandidateId { get; set; }
        public Guid TenantId { get; set; }
        public ApplicationStatus Status { get; set; }
        public DateTime DataInscricao { get; set; }
        public DateTime DataAtualizacaoStatus { get; set; }
        
        // Scores da IA
        public decimal? ScoreGeral { get; set; }
        public decimal? ScoreTecnico { get; set; }
        public decimal? ScoreExperiencia { get; set; }
        public decimal? ScoreCultural { get; set; }
        public decimal? ScoreSalario { get; set; }
        public string? JustificativaIA { get; set; }
        public string? PontosFortes { get; set; }
        public string? PontosAtencao { get; set; }
        public string? RecomendacaoIA { get; set; }
        public DateTime? DataCalculoScore { get; set; }
        
        // Avaliação manual
        public Guid? AvaliadoPor { get; set; }
        public string? NotasAvaliador { get; set; }
        public bool Favorito { get; set; }

        // Navigation properties
        public Job? Job { get; set; }
        public Candidate? Candidate { get; set; }
        public Tenant? Tenant { get; set; }
        public User? Avaliador { get; set; }
    }
}
