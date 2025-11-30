namespace LevverRH.Application.DTOs.Talents;

public class AnalyzeCandidateResponseDTO
{
    public int ScoreGeral { get; set; }
    public int ScoreTecnico { get; set; }
    public int ScoreExperiencia { get; set; }
    public int ScoreCultural { get; set; }
    public int ScoreSalario { get; set; }
    public string Justificativa { get; set; } = string.Empty;
    public string PontosFortes { get; set; } = string.Empty;
    public string PontosAtencao { get; set; } = string.Empty;
    public string Recomendacao { get; set; } = string.Empty;
    public int TokensUsed { get; set; }
    public decimal EstimatedCost { get; set; }
}
