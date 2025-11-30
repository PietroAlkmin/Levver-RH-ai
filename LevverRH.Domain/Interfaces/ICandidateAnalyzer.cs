namespace LevverRH.Domain.Interfaces;

public interface ICandidateAnalyzer
{
    Task<CandidateAnalysisResult> AnalyzeAsync(string resumeText, string jobRequirements);
}

public class CandidateAnalysisResult
{
    public int Score { get; set; }
    public string Summary { get; set; } = string.Empty;
    public int TokensUsed { get; set; }
    public decimal EstimatedCost { get; set; }
}
