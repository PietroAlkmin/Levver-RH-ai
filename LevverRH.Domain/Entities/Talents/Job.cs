using LevverRH.Domain.Enums.Talents;

namespace LevverRH.Domain.Entities.Talents;

/// <summary>
/// Entidade que representa uma vaga de emprego no sistema Talents
/// </summary>
public class Job
{
    // ========== IDENTIFICAÇÃO ==========
    
    /// <summary>
    /// Identificador único da vaga
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID do tenant (empresa) que criou a vaga
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// ID do usuário que criou a vaga
    /// </summary>
    public Guid CriadoPor { get; set; }

    // ========== INFORMAÇÕES BÁSICAS ==========
    
    /// <summary>
    /// Título da vaga (ex: "Desenvolvedor Full Stack Pleno")
    /// </summary>
    public string Titulo { get; set; } = string.Empty;

    /// <summary>
    /// Descrição completa da vaga
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Departamento/área da vaga (ex: "Tecnologia", "Comercial")
    /// </summary>
    public string? Departamento { get; set; }

    /// <summary>
    /// Número de vagas disponíveis (default 1)
    /// </summary>
    public int NumeroVagas { get; set; } = 1;

    // ========== LOCALIZAÇÃO E FORMATO ==========
    
    /// <summary>
    /// Localização completa (ex: "São Paulo - SP")
    /// </summary>
    public string? Localizacao { get; set; }

    /// <summary>
    /// Cidade onde a vaga está localizada
    /// </summary>
    public string? Cidade { get; set; }

    /// <summary>
    /// Estado (UF) onde a vaga está localizada
    /// </summary>
    public string? Estado { get; set; }

    /// <summary>
    /// Tipo de contrato (CLT, PJ, Estágio, Temporário)
    /// </summary>
    public ContractType? TipoContrato { get; set; }

    /// <summary>
    /// Modelo de trabalho (Presencial, Remoto, Híbrido)
    /// </summary>
    public WorkModel? ModeloTrabalho { get; set; }

    // ========== REQUISITOS ==========
    
    /// <summary>
    /// Anos mínimos de experiência necessários
    /// </summary>
    public int? AnosExperienciaMinimo { get; set; }

    /// <summary>
    /// Formação acadêmica necessária
    /// </summary>
    public string? FormacaoNecessaria { get; set; }

    /// <summary>
    /// Conhecimentos obrigatórios (JSON array)
    /// Ex: ["C#", "SQL", "Azure"]
    /// </summary>
    public string? ConhecimentosObrigatorios { get; set; }

    /// <summary>
    /// Conhecimentos desejáveis (JSON array)
    /// Ex: ["Docker", "Kubernetes"]
    /// </summary>
    public string? ConhecimentosDesejaveis { get; set; }

    /// <summary>
    /// Competências importantes (JSON array)
    /// Ex: ["Trabalho em equipe", "Comunicação"]
    /// </summary>
    public string? CompetenciasImportantes { get; set; }

    // ========== RESPONSABILIDADES ==========
    
    /// <summary>
    /// Descrição das atividades e responsabilidades do cargo
    /// </summary>
    public string? Responsabilidades { get; set; }

    // ========== REMUNERAÇÃO ==========
    
    /// <summary>
    /// Salário mínimo oferecido
    /// </summary>
    public decimal? SalarioMin { get; set; }

    /// <summary>
    /// Salário máximo oferecido
    /// </summary>
    public decimal? SalarioMax { get; set; }

    /// <summary>
    /// Benefícios oferecidos (ex: "VR, VT, Plano de Saúde")
    /// </summary>
    public string? Beneficios { get; set; }

    /// <summary>
    /// Informações sobre bônus e comissão
    /// </summary>
    public string? BonusComissao { get; set; }

    // ========== PROCESSO SELETIVO ==========
    
    /// <summary>
    /// Etapas do processo seletivo (JSON array)
    /// Ex: ["Triagem", "Entrevista RH", "Entrevista Técnica"]
    /// </summary>
    public string? EtapasProcesso { get; set; }

    /// <summary>
    /// Tipos de teste/entrevista (JSON array)
    /// Ex: ["Teste técnico", "Case de negócio"]
    /// </summary>
    public string? TiposTesteEntrevista { get; set; }

    /// <summary>
    /// Previsão de início da contratação
    /// </summary>
    public DateTime? PrevisaoInicio { get; set; }

    // ========== SOBRE A VAGA ==========
    
    /// <summary>
    /// Informações sobre o time/equipe
    /// </summary>
    public string? SobreTime { get; set; }

    /// <summary>
    /// Diferenciais da vaga/empresa
    /// </summary>
    public string? Diferenciais { get; set; }

    // ========== CONTROLE IA ==========
    
    /// <summary>
    /// ID da conversa com IA que criou/atualizou esta vaga
    /// </summary>
    public Guid? ConversationId { get; set; }

    /// <summary>
    /// Percentual de completude da vaga pela IA (0-100)
    /// </summary>
    public decimal? IaCompletionPercentage { get; set; }

    // ========== STATUS E AUDITORIA ==========
    
    /// <summary>
    /// Status atual da vaga
    /// </summary>
    public JobStatus Status { get; set; } = JobStatus.Rascunho;

    /// <summary>
    /// Data de criação da vaga
    /// </summary>
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data da última atualização
    /// </summary>
    public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data de fechamento da vaga (quando Status = Fechada)
    /// </summary>
    public DateTime? DataFechamento { get; set; }

    // ========== NAVEGAÇÃO ==========
    
    /// <summary>
    /// Navegação para o tenant
    /// </summary>
    public virtual Tenant? Tenant { get; set; }

    /// <summary>
    /// Navegação para o usuário que criou
    /// </summary>
    public virtual User? Criador { get; set; }

    /// <summary>
    /// Navegação para candidaturas desta vaga
    /// </summary>
    public virtual ICollection<Application>? Applications { get; set; }
}
