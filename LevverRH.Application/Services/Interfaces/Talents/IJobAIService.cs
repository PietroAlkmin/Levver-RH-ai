using LevverRH.Application.DTOs.Talents;
using LevverRH.Domain.Entities.Talents;

namespace LevverRH.Application.Services.Interfaces.Talents;

/// <summary>
/// Interface para serviço de criação de vagas assistida por IA
/// </summary>
public interface IJobAIService
{
    /// <summary>
    /// Gera a primeira pergunta para iniciar a criação da vaga
    /// </summary>
    /// <param name="mensagemInicial">Mensagem inicial do usuário (opcional)</param>
    /// <returns>Primeira pergunta da IA</returns>
    Task<string> GetFirstQuestionAsync(string? mensagemInicial = null);

    /// <summary>
    /// Processa a resposta do usuário e extrai dados para a vaga
    /// </summary>
    /// <param name="job">Vaga atual</param>
    /// <param name="conversationHistory">Histórico da conversa</param>
    /// <param name="userMessage">Mensagem do usuário</param>
    /// <returns>Resposta com dados extraídos e próxima pergunta</returns>
    Task<AIProcessingResult> ProcessUserResponseAsync(Job job, List<ChatMessageItem> conversationHistory, string userMessage);

    /// <summary>
    /// Calcula o percentual de completude da vaga
    /// </summary>
    Task<decimal> CalculateCompletionPercentageAsync(Job job);
}

/// <summary>
/// Resultado do processamento da IA
/// </summary>
public class AIProcessingResult
{
    /// <summary>
    /// Resposta/Próxima pergunta da IA
    /// </summary>
    public string AIResponse { get; set; } = string.Empty;

    /// <summary>
    /// Campos que foram atualizados com a resposta do usuário
    /// </summary>
    public Dictionary<string, object?> ExtractedFields { get; set; } = new();

    /// <summary>
    /// Lista de nomes dos campos atualizados
    /// </summary>
    public List<string> UpdatedFieldNames { get; set; } = new();

    /// <summary>
    /// Indica se a criação foi concluída (todas as perguntas respondidas)
    /// </summary>
    public bool IsComplete { get; set; }

    /// <summary>
    /// Percentual de completude
    /// </summary>
    public decimal CompletionPercentage { get; set; }
}

/// <summary>
/// Item do histórico de conversa
/// </summary>
public class ChatMessageItem
{
    public string Role { get; set; } = string.Empty; // "user" ou "assistant"
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
