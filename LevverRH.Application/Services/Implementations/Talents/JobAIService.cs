using LevverRH.Application.Services.Interfaces.Talents;
using LevverRH.Domain.Entities.Talents;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

// Alias para evitar conflito com LevverRH.Domain.Entities.Talents.ChatMessage
using AIChatMessage = Microsoft.Extensions.AI.ChatMessage;

namespace LevverRH.Application.Services.Implementations.Talents;

/// <summary>
/// Serviço de criação de vagas assistida por IA usando OpenAI GPT-4o-mini
/// </summary>
public class JobAIService : IJobAIService
{
    private readonly IChatClient _chatClient;
    private readonly IConfiguration _configuration;

    // Prompt do sistema para o assistente de criação de vagas
    private const string SYSTEM_PROMPT = @"
Você é um assistente especializado em Recursos Humanos da Levver, focado em ajudar recrutadores a criar vagas de emprego de forma eficiente e completa.

Seu objetivo é fazer perguntas conversacionais para coletar todas as informações necessárias para criar uma vaga completa. 

## Campos a coletar (em ordem sugerida):

### 1. Informações Básicas
- Título da vaga (ex: Desenvolvedor Full Stack Pleno)
- Área/Departamento (ex: Tecnologia, Comercial, RH)
- Número de vagas

### 2. Formato de Trabalho
- Tipo de contrato: CLT, PJ, Estagio, Temporario
- Modelo de trabalho: Presencial, Remoto, Hibrido
- Localização/Cidade/Estado (se presencial ou híbrido)

### 3. Requisitos
- Anos de experiência mínimo
- Formação necessária
- Conhecimentos obrigatórios (lista)
- Conhecimentos desejáveis (lista)
- Competências importantes (lista)

### 4. Responsabilidades
- O que a pessoa vai fazer no dia a dia

### 5. Remuneração
- Faixa salarial (mínimo e máximo)
- Benefícios
- Bônus/Comissão (se houver)

### 6. Processo Seletivo
- Etapas do processo (lista)
- Tipos de teste/entrevista (lista)
- Previsão de início

### 7. Sobre a Vaga (opcional)
- Sobre o time
- Diferenciais

## Regras:
1. Faça UMA pergunta por vez, de forma natural e conversacional
2. Após cada resposta do usuário, extraia os dados e pergunte o próximo campo
3. Se o usuário der uma resposta vaga, peça mais detalhes
4. Seja amigável e profissional
5. Quando todos os campos importantes forem preenchidos, informe que a vaga está pronta

## Formato de Resposta:
Sempre responda em JSON com a seguinte estrutura:
{
    ""message"": ""Sua pergunta ou mensagem para o usuário"",
    ""extractedFields"": {
        ""campo1"": ""valor1"",
        ""campo2"": [""item1"", ""item2""]
    },
    ""isComplete"": false,
    ""completionPercentage"": 25
}

Os campos extraídos devem usar os nomes exatos:
- titulo, descricao, departamento, numeroVagas
- tipoContrato (CLT/PJ/Estagio/Temporario), modeloTrabalho (Presencial/Remoto/Hibrido)
- localizacao, cidade, estado
- anosExperienciaMinimo, formacaoNecessaria
- conhecimentosObrigatorios, conhecimentosDesejaveis, competenciasImportantes (arrays)
- responsabilidades
- salarioMin, salarioMax, beneficios, bonusComissao
- etapasProcesso, tiposTesteEntrevista (arrays), previsaoInicio
- sobreTime, diferenciais
";

    public JobAIService(IChatClient chatClient, IConfiguration configuration)
    {
        _chatClient = chatClient;
        _configuration = configuration;
    }

    public async Task<string> GetFirstQuestionAsync(string? mensagemInicial = null)
    {
        var messages = new List<AIChatMessage>
        {
            new(ChatRole.System, SYSTEM_PROMPT)
        };

        if (!string.IsNullOrWhiteSpace(mensagemInicial))
        {
            messages.Add(new AIChatMessage(ChatRole.User, $"O usuário quer criar uma vaga e disse: \"{mensagemInicial}\". Extraia o que puder dessa mensagem e faça a próxima pergunta."));
        }
        else
        {
            messages.Add(new AIChatMessage(ChatRole.User, "O usuário quer criar uma nova vaga de emprego. Comece fazendo a primeira pergunta."));
        }

        var response = await _chatClient.GetResponseAsync(messages);
        var parsed = ParseAIResponse(response.Text);
        
        return parsed.Message;
    }

    public async Task<AIProcessingResult> ProcessUserResponseAsync(Job job, List<ChatMessageItem> conversationHistory, string userMessage)
    {
        var messages = new List<AIChatMessage>
        {
            new(ChatRole.System, SYSTEM_PROMPT)
        };

        // Adicionar contexto da vaga atual
        var currentJobContext = BuildJobContext(job);
        messages.Add(new AIChatMessage(ChatRole.System, $"Estado atual da vaga sendo criada:\n{currentJobContext}"));

        // Adicionar histórico da conversa
        foreach (var historyItem in conversationHistory)
        {
            var role = historyItem.Role == "user" ? ChatRole.User : ChatRole.Assistant;
            messages.Add(new AIChatMessage(role, historyItem.Content));
        }

        // Adicionar mensagem atual do usuário
        messages.Add(new AIChatMessage(ChatRole.User, userMessage));

        // Chamar a IA
        var response = await _chatClient.GetResponseAsync(messages);
        var parsed = ParseAIResponse(response.Text);

        return new AIProcessingResult
        {
            AIResponse = parsed.Message,
            ExtractedFields = parsed.ExtractedFields,
            UpdatedFieldNames = parsed.ExtractedFields.Keys.ToList(),
            IsComplete = parsed.IsComplete,
            CompletionPercentage = parsed.CompletionPercentage
        };
    }

    public Task<decimal> CalculateCompletionPercentageAsync(Job job)
    {
        var totalFields = 20; // Total de campos importantes
        var filledFields = 0;

        // Campos obrigatórios (peso maior)
        if (!string.IsNullOrWhiteSpace(job.Titulo)) filledFields += 2;
        if (!string.IsNullOrWhiteSpace(job.Descricao)) filledFields += 2;
        
        // Campos importantes
        if (!string.IsNullOrWhiteSpace(job.Departamento)) filledFields++;
        if (job.NumeroVagas > 0) filledFields++;
        if (job.TipoContrato.HasValue) filledFields++;
        if (job.ModeloTrabalho.HasValue) filledFields++;
        if (!string.IsNullOrWhiteSpace(job.Localizacao) || !string.IsNullOrWhiteSpace(job.Cidade)) filledFields++;
        if (job.AnosExperienciaMinimo.HasValue) filledFields++;
        if (!string.IsNullOrWhiteSpace(job.FormacaoNecessaria)) filledFields++;
        if (!string.IsNullOrWhiteSpace(job.ConhecimentosObrigatorios)) filledFields++;
        if (!string.IsNullOrWhiteSpace(job.ConhecimentosDesejaveis)) filledFields++;
        if (!string.IsNullOrWhiteSpace(job.CompetenciasImportantes)) filledFields++;
        if (!string.IsNullOrWhiteSpace(job.Responsabilidades)) filledFields++;
        if (job.SalarioMin.HasValue || job.SalarioMax.HasValue) filledFields++;
        if (!string.IsNullOrWhiteSpace(job.Beneficios)) filledFields++;
        if (!string.IsNullOrWhiteSpace(job.EtapasProcesso)) filledFields++;
        if (job.PrevisaoInicio.HasValue) filledFields++;

        var percentage = (decimal)filledFields / totalFields * 100;
        return Task.FromResult(Math.Min(100, Math.Round(percentage, 2)));
    }

    #region Helpers

    private string BuildJobContext(Job job)
    {
        var context = new Dictionary<string, object?>();

        if (!string.IsNullOrWhiteSpace(job.Titulo))
            context["titulo"] = job.Titulo;
        if (!string.IsNullOrWhiteSpace(job.Descricao))
            context["descricao"] = job.Descricao;
        if (!string.IsNullOrWhiteSpace(job.Departamento))
            context["departamento"] = job.Departamento;
        if (job.NumeroVagas > 0)
            context["numeroVagas"] = job.NumeroVagas;
        if (job.TipoContrato.HasValue)
            context["tipoContrato"] = job.TipoContrato.Value.ToString();
        if (job.ModeloTrabalho.HasValue)
            context["modeloTrabalho"] = job.ModeloTrabalho.Value.ToString();
        if (!string.IsNullOrWhiteSpace(job.Localizacao))
            context["localizacao"] = job.Localizacao;
        if (!string.IsNullOrWhiteSpace(job.Cidade))
            context["cidade"] = job.Cidade;
        if (!string.IsNullOrWhiteSpace(job.Estado))
            context["estado"] = job.Estado;
        if (job.AnosExperienciaMinimo.HasValue)
            context["anosExperienciaMinimo"] = job.AnosExperienciaMinimo;
        if (!string.IsNullOrWhiteSpace(job.FormacaoNecessaria))
            context["formacaoNecessaria"] = job.FormacaoNecessaria;
        if (!string.IsNullOrWhiteSpace(job.ConhecimentosObrigatorios))
            context["conhecimentosObrigatorios"] = job.ConhecimentosObrigatorios;
        if (!string.IsNullOrWhiteSpace(job.ConhecimentosDesejaveis))
            context["conhecimentosDesejaveis"] = job.ConhecimentosDesejaveis;
        if (!string.IsNullOrWhiteSpace(job.CompetenciasImportantes))
            context["competenciasImportantes"] = job.CompetenciasImportantes;
        if (!string.IsNullOrWhiteSpace(job.Responsabilidades))
            context["responsabilidades"] = job.Responsabilidades;
        if (job.SalarioMin.HasValue)
            context["salarioMin"] = job.SalarioMin;
        if (job.SalarioMax.HasValue)
            context["salarioMax"] = job.SalarioMax;
        if (!string.IsNullOrWhiteSpace(job.Beneficios))
            context["beneficios"] = job.Beneficios;
        if (!string.IsNullOrWhiteSpace(job.BonusComissao))
            context["bonusComissao"] = job.BonusComissao;
        if (!string.IsNullOrWhiteSpace(job.EtapasProcesso))
            context["etapasProcesso"] = job.EtapasProcesso;
        if (!string.IsNullOrWhiteSpace(job.TiposTesteEntrevista))
            context["tiposTesteEntrevista"] = job.TiposTesteEntrevista;
        if (job.PrevisaoInicio.HasValue)
            context["previsaoInicio"] = job.PrevisaoInicio;
        if (!string.IsNullOrWhiteSpace(job.SobreTime))
            context["sobreTime"] = job.SobreTime;
        if (!string.IsNullOrWhiteSpace(job.Diferenciais))
            context["diferenciais"] = job.Diferenciais;

        return JsonSerializer.Serialize(context, new JsonSerializerOptions { WriteIndented = true });
    }

    private AIResponseParsed ParseAIResponse(string? responseText)
    {
        if (string.IsNullOrWhiteSpace(responseText))
        {
            return new AIResponseParsed
            {
                Message = "Desculpe, não consegui processar sua resposta. Poderia repetir?",
                ExtractedFields = new Dictionary<string, object?>(),
                IsComplete = false,
                CompletionPercentage = 0
            };
        }

        try
        {
            // Tentar extrair JSON da resposta
            var jsonStart = responseText.IndexOf('{');
            var jsonEnd = responseText.LastIndexOf('}');

            if (jsonStart >= 0 && jsonEnd > jsonStart)
            {
                var jsonString = responseText.Substring(jsonStart, jsonEnd - jsonStart + 1);
                var parsed = JsonSerializer.Deserialize<JsonElement>(jsonString);

                var result = new AIResponseParsed
                {
                    Message = parsed.TryGetProperty("message", out var msg) ? msg.GetString() ?? "" : responseText,
                    IsComplete = parsed.TryGetProperty("isComplete", out var complete) && complete.GetBoolean(),
                    CompletionPercentage = parsed.TryGetProperty("completionPercentage", out var pct) ? pct.GetDecimal() : 0
                };

                if (parsed.TryGetProperty("extractedFields", out var fields))
                {
                    foreach (var field in fields.EnumerateObject())
                    {
                        result.ExtractedFields[field.Name] = ConvertJsonElement(field.Value);
                    }
                }

                return result;
            }
        }
        catch
        {
            // Se falhar o parse, retorna a mensagem como texto
        }

        return new AIResponseParsed
        {
            Message = responseText,
            ExtractedFields = new Dictionary<string, object?>(),
            IsComplete = false,
            CompletionPercentage = 0
        };
    }

    private object? ConvertJsonElement(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.TryGetInt32(out var intVal) ? intVal : element.GetDecimal(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Array => element.EnumerateArray().Select(ConvertJsonElement).ToList(),
            JsonValueKind.Null => null,
            _ => element.GetRawText()
        };
    }

    #endregion
}

internal class AIResponseParsed
{
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, object?> ExtractedFields { get; set; } = new();
    public bool IsComplete { get; set; }
    public decimal CompletionPercentage { get; set; }
}
