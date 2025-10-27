using LevverRH.Domain.Exceptions;

namespace LevverRH.Domain.Entities;

public class IntegrationCredentials
{
    public Guid Id { get; private set; }
  public Guid TenantId { get; private set; }
    public string Plataforma { get; private set; } = null!;
    public string Token { get; private set; } = null!;
    public string? RefreshToken { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public string? ConfiguracoesJson { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime DataAtualizacao { get; private set; }

 // Navigation
    public virtual Tenant Tenant { get; private set; } = null!;

    // EF Constructor
    private IntegrationCredentials() { }

    public IntegrationCredentials(
   Guid tenantId,
        string plataforma,
        string token,
        Tenant tenant,
        string? refreshToken = null,
        DateTime? expiresAt = null,
   string? configuracoesJson = null)
    {
 if (string.IsNullOrWhiteSpace(plataforma))
  throw new DomainException("Plataforma é obrigatória.");

        if (string.IsNullOrWhiteSpace(token))
 throw new DomainException("Token é obrigatório.");

    if (tenant == null)
          throw new DomainException("Tenant não existe.");

        Id = Guid.NewGuid();
 TenantId = tenantId;
        Plataforma = plataforma;
        Token = token;
        RefreshToken = refreshToken;
 ExpiresAt = expiresAt;
        ConfiguracoesJson = configuracoesJson;
        Ativo = true;
        DataCriacao = DateTime.UtcNow;
        DataAtualizacao = DateTime.UtcNow;
        Tenant = tenant;
    }

    public void Ativar()
    {
 Ativo = true;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Desativar()
    {
        Ativo = false;
        DataAtualizacao = DateTime.UtcNow;
    }

public void AtualizarToken(string token, string? refreshToken = null, DateTime? expiresAt = null)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new DomainException("Token é obrigatório.");

  Token = token;
        RefreshToken = refreshToken;
        ExpiresAt = expiresAt;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void AtualizarConfiguracoes(string? configuracoesJson)
    {
        ConfiguracoesJson = configuracoesJson;
      DataAtualizacao = DateTime.UtcNow;
    }

    public bool TokenExpirado()
    {
        return ExpiresAt.HasValue && ExpiresAt.Value <= DateTime.UtcNow;
    }
}
