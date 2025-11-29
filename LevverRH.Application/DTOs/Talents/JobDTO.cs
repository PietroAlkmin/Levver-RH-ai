using LevverRH.Domain.Enums.Talents;

namespace LevverRH.Application.DTOs.Talents;

#region Response DTOs

/// <summary>
/// DTO resumido para listagem de vagas
/// </summary>
public class JobDTO
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string? Departamento { get; set; }
    public string? Localizacao { get; set; }
    public string? TipoContrato { get; set; }
    public string? ModeloTrabalho { get; set; }
    public decimal? SalarioMin { get; set; }
    public decimal? SalarioMax { get; set; }
    public string? Beneficios { get; set; }
    public string Status { get; set; } = string.Empty;
    public int NumeroVagas { get; set; }
    public DateTime DataCriacao { get; set; }
    public int TotalCandidaturas { get; set; }
    public decimal? IaCompletionPercentage { get; set; }
}

/// <summary>
/// DTO completo com todos os campos da vaga
/// </summary>
public class JobDetailDTO
{
    // Identificação
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid CriadoPor { get; set; }

    // Informações Básicas
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string? Departamento { get; set; }
    public int NumeroVagas { get; set; }

    // Localização e Formato
    public string? Localizacao { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? TipoContrato { get; set; }
    public string? ModeloTrabalho { get; set; }

    // Requisitos
    public int? AnosExperienciaMinimo { get; set; }
    public string? FormacaoNecessaria { get; set; }
    public List<string>? ConhecimentosObrigatorios { get; set; }
    public List<string>? ConhecimentosDesejaveis { get; set; }
    public List<string>? CompetenciasImportantes { get; set; }

    // Responsabilidades
    public string? Responsabilidades { get; set; }

    // Remuneração
    public decimal? SalarioMin { get; set; }
    public decimal? SalarioMax { get; set; }
    public string? Beneficios { get; set; }
    public string? BonusComissao { get; set; }

    // Processo Seletivo
    public List<string>? EtapasProcesso { get; set; }
    public List<string>? TiposTesteEntrevista { get; set; }
    public DateTime? PrevisaoInicio { get; set; }

    // Sobre a Vaga
    public string? SobreTime { get; set; }
    public string? Diferenciais { get; set; }

    // Controle IA
    public Guid? ConversationId { get; set; }
    public decimal? IaCompletionPercentage { get; set; }

    // Status e Auditoria
    public string Status { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
    public DateTime DataAtualizacao { get; set; }
    public DateTime? DataFechamento { get; set; }

    // Contadores
    public int TotalCandidaturas { get; set; }
}

#endregion

#region Request DTOs - Criação Manual

/// <summary>
/// DTO para criação manual de vaga (sem IA)
/// </summary>
public class CreateJobDTO
{
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string? Departamento { get; set; }
    public string? Localizacao { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public ContractType? TipoContrato { get; set; }
    public WorkModel? ModeloTrabalho { get; set; }
    public decimal? SalarioMin { get; set; }
    public decimal? SalarioMax { get; set; }
    public string? Beneficios { get; set; }
    public int NumeroVagas { get; set; } = 1;
}

/// <summary>
/// DTO para atualização manual de vaga
/// </summary>
public class UpdateJobDTO
{
    public string? Titulo { get; set; }
    public string? Descricao { get; set; }
    public string? Departamento { get; set; }
    public string? Localizacao { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public ContractType? TipoContrato { get; set; }
    public WorkModel? ModeloTrabalho { get; set; }
    public decimal? SalarioMin { get; set; }
    public decimal? SalarioMax { get; set; }
    public string? Beneficios { get; set; }
    public string? BonusComissao { get; set; }
    public int? AnosExperienciaMinimo { get; set; }
    public string? FormacaoNecessaria { get; set; }
    public List<string>? ConhecimentosObrigatorios { get; set; }
    public List<string>? ConhecimentosDesejaveis { get; set; }
    public List<string>? CompetenciasImportantes { get; set; }
    public string? Responsabilidades { get; set; }
    public List<string>? EtapasProcesso { get; set; }
    public List<string>? TiposTesteEntrevista { get; set; }
    public DateTime? PrevisaoInicio { get; set; }
    public string? SobreTime { get; set; }
    public string? Diferenciais { get; set; }
    public int? NumeroVagas { get; set; }
    public JobStatus? Status { get; set; }
}

#endregion

#region Request DTOs - Criação com IA

/// <summary>
/// DTO para iniciar criação de vaga com IA
/// </summary>
public class StartJobCreationDTO
{
    /// <summary>
    /// Mensagem inicial do usuário (opcional)
    /// Ex: "Quero criar uma vaga de desenvolvedor"
    /// </summary>
    public string? MensagemInicial { get; set; }
}

/// <summary>
/// DTO para enviar mensagem no chat de criação de vaga
/// </summary>
public class JobChatMessageDTO
{
    /// <summary>
    /// ID da vaga sendo criada
    /// </summary>
    public Guid JobId { get; set; }

    /// <summary>
    /// Mensagem do usuário
    /// </summary>
    public string Mensagem { get; set; } = string.Empty;
}

/// <summary>
/// DTO de resposta do chat de criação de vaga
/// </summary>
public class JobChatResponseDTO
{
    /// <summary>
    /// ID da vaga
    /// </summary>
    public Guid JobId { get; set; }

    /// <summary>
    /// ID da conversa
    /// </summary>
    public Guid ConversationId { get; set; }

    /// <summary>
    /// Resposta da IA
    /// </summary>
    public string MensagemIA { get; set; } = string.Empty;

    /// <summary>
    /// Campos que foram atualizados com esta resposta
    /// </summary>
    public List<string> CamposAtualizados { get; set; } = new();

    /// <summary>
    /// Percentual de completude da vaga (0-100)
    /// </summary>
    public decimal CompletionPercentage { get; set; }

    /// <summary>
    /// Indica se a criação foi concluída
    /// </summary>
    public bool CriacaoConcluida { get; set; }

    /// <summary>
    /// Dados atualizados da vaga
    /// </summary>
    public JobDetailDTO? JobAtualizado { get; set; }
}

/// <summary>
/// DTO para finalizar criação de vaga
/// </summary>
public class CompleteJobCreationDTO
{
    /// <summary>
    /// ID da vaga
    /// </summary>
    public Guid JobId { get; set; }

    /// <summary>
    /// Se deve publicar a vaga imediatamente (Status = Aberta)
    /// </summary>
    public bool PublicarImediatamente { get; set; } = false;
}

#endregion
