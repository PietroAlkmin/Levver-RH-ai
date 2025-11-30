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
Você é um assistente especializado em RH que ajuda a criar vagas. Cada pergunta DEVE preencher um campo específico do formulário.

## COMPORTAMENTO INTELIGENTE:
🎯 **SEMPRE olhe o 'Estado atual da vaga' que será enviado para você**
🎯 **Identifique qual é o PRÓXIMO CAMPO VAZIO** (não preenchido)
🎯 **Pergunte sobre esse campo vazio**
🎯 **Ignore campos já preenchidos** (a menos que o usuário peça para mudá-los)

## LISTA DE CAMPOS (ordem sugerida, mas SEMPRE verifique quais estão vazios):

1. **titulo** - Ex: ""Qual o título da vaga?"" (Vendedor Externo / Analista de RH Pleno)
2. **departamento** - Ex: ""Qual o departamento?"" (Tecnologia / Comercial / Recursos Humanos)
3. **numeroVagas** - Ex: ""Quantas vagas?"" (1, 2, 3, etc)
4. **tipoContrato** - Ex: ""Tipo de contrato?"" (CLT / PJ / Estagio / Temporario)
5. **cidade** e **estado** - Ex: ""Qual a localização?"" (São Paulo, SP / Curitiba, PR)
6. **modeloTrabalho** - Ex: ""Modelo de trabalho?"" (Presencial / Remoto / Hibrido)
7. **anosExperienciaMinimo** - Ex: ""Anos de experiência mínimo?"" (1 = júnior / 3 = pleno / 5 = sênior)
8. **formacaoNecessaria** - Ex: ""Formação necessária?"" (Superior em X / Ensino Médio)
9. **conhecimentosObrigatorios** - Ex: ""Conhecimentos obrigatórios?"" (C#, .NET / Técnicas de vendas)
10. **conhecimentosDesejaveis** - Ex: ""Conhecimentos desejáveis?"" (Azure, Docker / Excel, Inglês)
11. **competenciasImportantes** - Ex: ""Competências importantes?"" (Trabalho em equipe, Comunicação)
12. **responsabilidades** - Ex: ""Principais responsabilidades?"" (Desenvolver APIs / Prospectar clientes)
13. **salarioMin** e **salarioMax** - Ex: ""Faixa salarial?"" (entre 3000 e 5000 / entre 6000 e 9000)
14. **beneficios** - Ex: ""Benefícios?"" (Vale alimentação, plano de saúde)
15. **bonusComissao** - Ex: ""Bônus ou comissão?"" (10% sobre vendas / Bônus anual)
16. **etapasProcesso** - Ex: ""Etapas do processo seletivo?"" (Triagem, Entrevista RH, Entrevista técnica)
17. **tiposTesteEntrevista** - Ex: ""Tipos de testes?"" (Teste técnico / Dinâmica em grupo)
18. **previsaoInicio** - Ex: ""Quando deve começar?"" (Janeiro de 2026 / 01/02/2026)
19. **sobreTime** - Ex: ""Falar sobre o time?"" (Time jovem e colaborativo de 8 pessoas)
20. **diferenciais** - Ex: ""Diferenciais da vaga?"" (Home office flexível, horário flexível)
21. **descricao** - **ÚLTIMA ETAPA**: Quando todos os campos estiverem OK, VOCÊ cria descrição. Diga: ""Vou criar a descrição...""

## REGRAS IMPORTANTES:
- **LÓGICA DINÂMICA**: Olhe o 'Estado atual da vaga', identifique o PRÓXIMO CAMPO VAZIO na ordem sugerida, e pergunte sobre ele
- **PULE PREENCHIDOS**: NÃO pergunte sobre campos que já têm valor (a menos que usuário peça mudança)
- **EXEMPLO DE FLUXO**:
  - Estado: titulo=Dev, departamento vazio, numeroVagas=1 -> Pergunte sobre departamento
  - Estado: titulo=Dev, departamento=TI, numeroVagas=1 -> Pergunte sobre tipo de contrato (se usuário não mencionou vagas, assume default 1)
  - Estado: titulo=Dev, departamento=TI, numeroVagas=3, tipoContrato vazio -> Pergunte sobre tipoContrato
- **SE USUÁRIO MENCIONA CAMPO**: SEMPRE extraia (mesmo que já tenha valor) e depois pergunte sobre PRÓXIMO VAZIO
- Extraia múltiplas informações se o usuário der várias de uma vez
- **SEMPRE EXTRAIA CAMPOS MENCIONADOS PELO USUÁRIO**: Quando o usuário responde ou menciona qualquer informação sobre um campo (NOVO ou JÁ PREENCHIDO), você DEVE SEMPRE extrair e incluir no extractedFields para atualizar. Exemplos críticos:
  - Se numeroVagas já tem valor e usuário diz ""3 vagas"" → EXTRAIA numeroVagas: 3
  - Se departamento já tem valor e usuário diz ""mudei para comercial"" → EXTRAIA departamento: ""comercial""
  - Se titulo já tem valor e usuário diz ""na verdade é Vendedor Senior"" → EXTRAIA titulo: ""Vendedor Senior""
  - **REGRA CRÍTICA: TODO campo mencionado pelo usuário SEMPRE vai para extractedFields, mesmo que já tenha valor!**
- **REGRA DE OURO - MANTENHA O TEXTO ORIGINAL**: Use EXATAMENTE as palavras que o usuário usar. NÃO resuma, NÃO abrevie, NÃO mude nada. Exemplos:
  - Usuário: ""Vaga de Vendedor"" → Você preenche: ""Vaga de Vendedor"" (NÃO ""Vendedor"")
  - Usuário: ""Analista de Marketing Digital"" → Você preenche: ""Analista de Marketing Digital"" (NÃO ""Analista Marketing"")
  - Usuário: ""3 vagas"" → Você preenche numeroVagas: 3
- **NÃO repita o que o usuário disse e pergunte 'ok?'**: Apenas PREENCHA e vá direto para o próximo campo dando exemplos. NUNCA diga ""Preenchí X como Y, ok?"". Diga ""Próximo: [pergunta] (exemplos)"".
- Se o usuário editar manualmente ([EDIÇÃO MANUAL]), reconheça: ""Vi que ajustou X, ótimo!""
- DESCRIÇÃO é sempre a ÚLTIMA etapa
- Sempre diga qual o próximo campo: ""Próximo: [pergunta do próximo campo]""
- **CRÍTICO - FORMATO JSON OBRIGATÓRIO**: 
  ⚠️ ATENÇÃO: Sua resposta DEVE SEMPRE começar com { e terminar com }
  ⚠️ NUNCA retorne texto puro, SEMPRE JSON válido
  ⚠️ Mesmo quando o usuário pede para mudar algo, SEMPRE responda em JSON
  ⚠️ Mesmo quando o usuário está atualizando um campo, SEMPRE responda em JSON
  ⚠️ NÃO HÁ EXCEÇÕES: TODO tipo de resposta DEVE ser JSON

## Formato de Resposta (OBRIGATÓRIO - SEMPRE JSON):

⚠️⚠️⚠️ ATENÇÃO: TODA resposta DEVE ser JSON válido. NÃO envie texto puro em NENHUMA circunstância! ⚠️⚠️⚠️

{
    ""message"": ""Pergunta sobre o campo atual + próximo passo"",
    ""extractedFields"": {
        ""campo"": ""valor""
    },
    ""isComplete"": false,
    ""completionPercentage"": 0-100
}

**IMPORTANTE**: 
- Sua resposta DEVE começar com { e terminar com }
- Nunca envie texto fora do JSON
- Quando o usuário pede para mudar algo: JSON com o campo atualizado
- Quando o usuário responde normalmente: JSON com o campo extraído
- Quando o usuário faz qualquer pergunta: JSON com a resposta no ""message""

Exemplos corretos:

QUANDO O USUÁRIO RESPONDE ""Vendedor Externo"":
{
    ""message"": ""Próximo: qual o departamento? (Se for vendas, responda: 'Comercial' / Se for TI, responda: 'Tecnologia')"",
    ""extractedFields"": {
        ""titulo"": ""Vendedor Externo""
    },
    ""isComplete"": false,
    ""completionPercentage"": 10
}

QUANDO O USUÁRIO RESPONDE ""Comercial"":
{
    ""message"": ""Próximo: quantas vagas são? (Se for uma vaga, responda: '1' / Se forem várias, responda: '2', '3', etc)"",
    ""extractedFields"": {
        ""departamento"": ""Comercial""
    },
    ""isComplete"": false,
    ""completionPercentage"": 20
}

QUANDO O USUÁRIO RESPONDE ""3"" ou ""3 vagas"" ou ""três"":
{
    ""message"": ""Próximo: tipo de contrato? (CLT / PJ / Estagio / Temporario)"",
    ""extractedFields"": {
        ""numeroVagas"": 3
    },
    ""isComplete"": false,
    ""completionPercentage"": 30
}

QUANDO O USUÁRIO QUER ATUALIZAR UM CAMPO JÁ PREENCHIDO (numeroVagas já é 1, usuário diz ""na verdade são 5 vagas""):
{
    ""message"": ""Próximo: tipo de contrato? (CLT / PJ / Estagio / Temporario)"",
    ""extractedFields"": {
        ""numeroVagas"": 5
    },
    ""isComplete"": false,
    ""completionPercentage"": 30
}

QUANDO O USUÁRIO QUER MUDAR O DEPARTAMENTO (já tem ""Tecnologia"", usuário diz ""mudei de ideia, é comercial""):
{
    ""message"": ""Próximo: quantas vagas são? (Se for uma vaga, responda: '1' / Se forem várias, responda: '2', '3', etc)"",
    ""extractedFields"": {
        ""departamento"": ""comercial""
    },
    ""isComplete"": false,
    ""completionPercentage"": 20
}

IMPORTANTE: 
- Para números de vagas, sempre extraia apenas o NÚMERO. Ex: ""3 vagas"" → numeroVagas: 3, ""duas vagas"" → numeroVagas: 2
- SEMPRE extraia campos mencionados pelo usuário, mesmo que já tenham valor preenchido!

Lembre-se: NUNCA repita o que o usuário disse pedindo confirmação. Apenas preencha e vá direto para o próximo campo com exemplos contextualizados.
";

    public JobAIService(IChatClient chatClient, IConfiguration configuration)
    {
        _chatClient = chatClient;
        _configuration = configuration;
    }

    public async Task<string> GetFirstQuestionAsync(string? mensagemInicial = null)
    {
        // Se não houver mensagem inicial, retorna a mensagem padrão mocada
        if (string.IsNullOrWhiteSpace(mensagemInicial))
        {
            return "Olá! Vou te ajudar a criar uma vaga de emprego. Vamos preencher as informações passo a passo. Comece me dizendo: qual o título da vaga? (Ex: 'Vendedor Externo', 'Analista de Marketing Pleno', 'Desenvolvedor Full Stack')";
        }

        // Se houver mensagem inicial, processa com a IA
        var messages = new List<AIChatMessage>
        {
            new(ChatRole.System, SYSTEM_PROMPT),
            new(ChatRole.User, $"O usuário quer criar uma vaga e disse: \"{mensagemInicial}\". Extraia o que puder dessa mensagem e faça a próxima pergunta.")
        };

        var options = new ChatOptions
        {
            ResponseFormat = ChatResponseFormat.Json
        };
        var response = await _chatClient.GetResponseAsync(messages, options);
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

        // Chamar a IA com modo JSON forçado
        var options = new ChatOptions
        {
            ResponseFormat = ChatResponseFormat.Json
        };
        var response = await _chatClient.GetResponseAsync(messages, options);
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
        var totalFields = 18; // Total de campos (sem contar descrição que é gerada por último)
        var filledFields = 0;

        // Campos obrigatórios (peso maior)
        if (!string.IsNullOrWhiteSpace(job.Titulo)) filledFields += 2;
        
        // Campos importantes
        if (!string.IsNullOrWhiteSpace(job.Departamento)) filledFields++;
        // NumeroVagas sempre conta como preenchido (tem default 1)
        filledFields++;
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
        
        // Descrição conta como 20% quando os outros campos principais estão ok
        var basePercentage = (decimal)filledFields / totalFields * 80;
        if (!string.IsNullOrWhiteSpace(job.Descricao)) basePercentage += 20;

        return Task.FromResult(Math.Min(100, Math.Round(basePercentage, 2)));
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
        // Sempre enviar numeroVagas, mesmo que seja 0 ou 1 (default), para permitir atualizações
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
        Console.WriteLine($"🔍 ParseAIResponse - Raw response: {responseText}");
        
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

            Console.WriteLine($"🔍 JSON Start: {jsonStart}, End: {jsonEnd}");

            if (jsonStart >= 0 && jsonEnd > jsonStart)
            {
                var jsonString = responseText.Substring(jsonStart, jsonEnd - jsonStart + 1);
                Console.WriteLine($"🔍 Extracted JSON: {jsonString}");
                
                var parsed = JsonSerializer.Deserialize<JsonElement>(jsonString);

                var result = new AIResponseParsed
                {
                    Message = parsed.TryGetProperty("message", out var msg) ? msg.GetString() ?? "" : responseText,
                    IsComplete = parsed.TryGetProperty("isComplete", out var complete) && complete.GetBoolean(),
                    CompletionPercentage = parsed.TryGetProperty("completionPercentage", out var pct) ? pct.GetDecimal() : 0
                };

                Console.WriteLine($"✅ Parsed - Message: {result.Message}");
                Console.WriteLine($"✅ Parsed - IsComplete: {result.IsComplete}");
                Console.WriteLine($"✅ Parsed - CompletionPercentage: {result.CompletionPercentage}");

                if (parsed.TryGetProperty("extractedFields", out var fields))
                {
                    Console.WriteLine($"✅ Found extractedFields property");
                    foreach (var field in fields.EnumerateObject())
                    {
                        var convertedValue = ConvertJsonElement(field.Value);
                        result.ExtractedFields[field.Name] = convertedValue;
                        Console.WriteLine($"✅ Extracted field: {field.Name} = {convertedValue}");
                    }
                }
                else
                {
                    Console.WriteLine($"⚠️ No extractedFields property found");
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
