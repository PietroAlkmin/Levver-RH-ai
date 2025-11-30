namespace LevverRH.Application.DTOs.Public;

/// <summary>
/// DTO para candidatura pública (sem autenticação)
/// </summary>
public class CreatePublicApplicationDTO
{
    /// <summary>
    /// ID da vaga para qual está se candidatando
    /// </summary>
    public Guid JobId { get; set; }

    /// <summary>
    /// Nome completo do candidato
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Email do candidato
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Telefone do candidato (com DDD)
    /// </summary>
    public string Telefone { get; set; } = string.Empty;

    /// <summary>
    /// LinkedIn (opcional)
    /// </summary>
    public string? LinkedinUrl { get; set; }

    /// <summary>
    /// Deseja criar conta para acompanhar candidaturas
    /// </summary>
    public bool CriarConta { get; set; } = false;

    /// <summary>
    /// Senha (obrigatório se CriarConta = true)
    /// </summary>
    public string? Senha { get; set; }
}

/// <summary>
/// Resposta da candidatura pública
/// </summary>
public class PublicApplicationResponseDTO
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? ApplicationId { get; set; }
    public bool ContaCriada { get; set; } = false;
    public string? AccessToken { get; set; }
}

/// <summary>
/// DTO público com detalhes da vaga (sem dados sensíveis)
/// </summary>
public class PublicJobDetailDTO
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string? Departamento { get; set; }
    public string? Localizacao { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? TipoContrato { get; set; }
    public string? ModeloTrabalho { get; set; }
    public decimal? SalarioMin { get; set; }
    public decimal? SalarioMax { get; set; }
    public string? Beneficios { get; set; }
    public int? AnosExperienciaMinimo { get; set; }
    public string? FormacaoNecessaria { get; set; }
    public string? Responsabilidades { get; set; }
    public string? SobreTime { get; set; }
    public string? Diferenciais { get; set; }
    public DateTime DataCriacao { get; set; }
    
    // Informações da empresa (white label)
    public string NomeEmpresa { get; set; } = string.Empty;
    public string? LogoEmpresa { get; set; }
}
