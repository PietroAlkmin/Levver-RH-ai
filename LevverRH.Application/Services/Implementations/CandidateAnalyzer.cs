using LevverRH.Domain.Interfaces;
using Microsoft.Extensions.AI;
using System.Text.Json;

// Alias para evitar conflito
using AIChatMessage = Microsoft.Extensions.AI.ChatMessage;

namespace LevverRH.Application.Services.Implementations;

public class CandidateAnalyzer : ICandidateAnalyzer
{
    private readonly IChatClient _chatClient;
    private const string DefaultModel = "gpt-4o";

    public CandidateAnalyzer(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<CandidateAnalysisResult> AnalyzeAsync(string resumeText, string jobRequirements)
    {
        var systemPrompt = CreateSystemPrompt();
        var userPrompt = CreateUserPrompt(resumeText, jobRequirements);

        var messages = new List<AIChatMessage>
        {
            new(ChatRole.System, systemPrompt),
            new(ChatRole.User, userPrompt)
        };

        var chatOptions = new ChatOptions
        {
            ResponseFormat = ChatResponseFormat.Json,
            Temperature = 0.1f,
            MaxOutputTokens = 4096
        };

        var response = await _chatClient.GetResponseAsync(messages, chatOptions);
        var tokensUsed = (int)(response.Usage?.TotalTokenCount ?? 0);
        var estimatedCost = CalculateCost(tokensUsed);

        var result = ParseResponse(response.Text);
        result.TokensUsed = tokensUsed;
        result.EstimatedCost = estimatedCost;

        return result;
    }

    private string CreateSystemPrompt()
    {
        return @"Você é um especialista em análise de currículos e recrutamento.
Sua tarefa é analisar o currículo de um candidato comparando-o com os requisitos da vaga.

Você deve retornar um JSON com a seguinte estrutura:
{
  ""scoreGeral"": 85,
  ""scoreTecnico"": 90,
  ""scoreExperiencia"": 80,
  ""scoreCultural"": 85,
  ""scoreSalario"": 75,
  ""justificativa"": ""Resumo geral da análise..."",
  ""pontosFortes"": ""Principais pontos positivos..."",
  ""pontosAtencao"": ""Pontos que merecem atenção..."",
  ""recomendacao"": ""Recomendado/Considerar/Não Recomendado""
}

Critérios de pontuação (0-100):
- Score Técnico: Habilidades técnicas requeridas vs apresentadas
- Score Experiência: Anos e qualidade da experiência profissional
- Score Cultural: Alinhamento com valores e cultura da empresa (quando mencionados)
- Score Salário: Compatibilidade da pretensão salarial (se disponível)
- Score Geral: Média ponderada dos scores acima

Seja objetivo, profissional e baseie-se apenas nas informações fornecidas.";
    }

    private string CreateUserPrompt(string resumeText, string jobRequirements)
    {
        return $@"REQUISITOS DA VAGA:
{jobRequirements}

CURRÍCULO DO CANDIDATO:
{resumeText}

Por favor, analise o candidato e retorne o JSON com os scores e análise.";
    }

    private CandidateAnalysisResult ParseResponse(string? responseText)
    {
        if (string.IsNullOrWhiteSpace(responseText))
        {
            return new CandidateAnalysisResult
            {
                Score = 0,
                Summary = "Erro: Resposta vazia da IA"
            };
        }

        try
        {
            var jsonDoc = JsonDocument.Parse(responseText);
            var root = jsonDoc.RootElement;

            return new CandidateAnalysisResult
            {
                Score = root.GetProperty("scoreGeral").GetInt32(),
                Summary = root.GetProperty("justificativa").GetString() ?? string.Empty
            };
        }
        catch (Exception ex)
        {
            return new CandidateAnalysisResult
            {
                Score = 0,
                Summary = $"Erro ao processar resposta: {ex.Message}"
            };
        }
    }

    private decimal CalculateCost(int tokens)
    {
        // GPT-4o pricing: ~$2.50/1M input tokens, ~$10/1M output tokens
        // Estimativa simplificada: $5/1M tokens (média)
        const decimal costPerMillionTokens = 5.00m;
        return (tokens / 1_000_000m) * costPerMillionTokens;
    }
}
