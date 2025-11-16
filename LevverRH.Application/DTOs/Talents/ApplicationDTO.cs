namespace LevverRH.Application.DTOs.Talents
{
    public class ApplicationDTO
    {
        public Guid Id { get; set; }
        public Guid JobId { get; set; }
        public string JobTitulo { get; set; } = string.Empty;
        public Guid CandidateId { get; set; }
        public string CandidateNome { get; set; } = string.Empty;
        public string CandidateEmail { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime DataInscricao { get; set; }
        public decimal? ScoreGeral { get; set; }
        public bool Favorito { get; set; }
    }

    public class ApplicationDetailDTO
    {
        public Guid Id { get; set; }
        public JobDTO? Job { get; set; }
        public CandidateDTO? Candidate { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime DataInscricao { get; set; }
        public decimal? ScoreGeral { get; set; }
        public decimal? ScoreTecnico { get; set; }
        public decimal? ScoreExperiencia { get; set; }
        public decimal? ScoreCultural { get; set; }
        public string? JustificativaIA { get; set; }
        public string? PontosFortes { get; set; }
        public string? PontosAtencao { get; set; }
        public string? NotasAvaliador { get; set; }
        public bool Favorito { get; set; }
    }
}
