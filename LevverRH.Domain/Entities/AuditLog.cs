namespace LevverRH.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid? UserId { get; private set; }
    public string Acao { get; private set; } = null!;
    public string? Entidade { get; private set; }
    public Guid? EntidadeId { get; private set; }
    public string? DetalhesJson { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public DateTime DataHora { get; private set; }

    // EF Constructor
    private AuditLog() { }

    // Factory Methods
    public static AuditLog CriarLogLogin(Guid tenantId, Guid userId, string ipAddress, string? userAgent)
    {
        return new AuditLog
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = userId,
            Acao = "user_login",
            IpAddress = ipAddress,
            UserAgent = userAgent,
            DataHora = DateTime.UtcNow
        };
    }

    public static AuditLog CriarLogLogout(Guid tenantId, Guid userId, string ipAddress)
    {
        return new AuditLog
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = userId,
            Acao = "user_logout",
            IpAddress = ipAddress,
            DataHora = DateTime.UtcNow
        };
    }

    public static AuditLog CriarLogMudancaPermissao(
        Guid tenantId,
        Guid userId,
        Guid usuarioAfetadoId,
        string detalhes,
        string ipAddress)
    {
        return new AuditLog
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = userId,
            Acao = "role_changed",
            Entidade = "user",
            EntidadeId = usuarioAfetadoId,
            DetalhesJson = detalhes,
            IpAddress = ipAddress,
            DataHora = DateTime.UtcNow
        };
    }

    public static AuditLog CriarLogVisualizaçãoCurrículo(
        Guid tenantId,
        Guid userId,
        Guid candidateId,
        string ipAddress)
    {
        return new AuditLog
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = userId,
            Acao = "curriculo_visualizado",
            Entidade = "candidate",
            EntidadeId = candidateId,
            IpAddress = ipAddress,
            DataHora = DateTime.UtcNow
        };
    }

    public static AuditLog CriarLogDownloadCurrículo(
        Guid tenantId,
        Guid userId,
        Guid candidateId,
        string ipAddress)
    {
        return new AuditLog
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = userId,
            Acao = "curriculo_download",
            Entidade = "candidate",
            EntidadeId = candidateId,
            IpAddress = ipAddress,
            DataHora = DateTime.UtcNow
        };
    }

    public static AuditLog CriarLogExportaçãoDados(
        Guid tenantId,
        Guid userId,
        string entidade,
        string formato,
        string ipAddress)
    {
        return new AuditLog
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = userId,
            Acao = "dados_exportados",
            Entidade = entidade,
            DetalhesJson = $"{{\"formato\": \"{formato}\"}}",
            IpAddress = ipAddress,
            DataHora = DateTime.UtcNow
        };
    }

    public static AuditLog CriarLogTentativaLoginFalhou(
        Guid tenantId,
        string email,
        string ipAddress,
        string motivo)
    {
        return new AuditLog
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Acao = "login_falhou",
            DetalhesJson = $"{{\"email\": \"{email}\", \"motivo\": \"{motivo}\"}}",
            IpAddress = ipAddress,
            DataHora = DateTime.UtcNow
        };
    }

    public static AuditLog CriarLogTenantMudancaStatus(
        Guid tenantId,
        string statusAnterior,
        string statusNovo,
        Guid userId)
    {
        return new AuditLog
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = userId,
            Acao = "tenant_status_changed",
            Entidade = "tenant",
            EntidadeId = tenantId,
            DetalhesJson = $"{{\"de\": \"{statusAnterior}\", \"para\": \"{statusNovo}\"}}",
            DataHora = DateTime.UtcNow
        };
    }

    public static AuditLog CriarLogSubscriptionMudancaStatus(
        Guid subscriptionId,
        Guid tenantId,
        string statusAnterior,
        string statusNovo,
        Guid userId)
    {
        return new AuditLog
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = userId,
            Acao = "subscription_status_changed",
            Entidade = "tenant_subscription",
            EntidadeId = subscriptionId,
            DetalhesJson = $"{{\"de\": \"{statusAnterior}\", \"para\": \"{statusNovo}\"}}",
            DataHora = DateTime.UtcNow
        };
    }

    public static AuditLog CriarLogProdutoMudancaPreco(
        Guid productId,
        decimal precoAnterior,
        decimal precoNovo,
        Guid userId)
    {
        return new AuditLog
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.Empty,
            UserId = userId,
            Acao = "product_price_changed",
            Entidade = "product_catalog",
            EntidadeId = productId,
            DetalhesJson = $"{{\"de\": {precoAnterior}, \"para\": {precoNovo}}}",
            DataHora = DateTime.UtcNow
        };
    }

    public static AuditLog CriarLogGenerico(
        Guid tenantId,
        Guid? userId,
        string acao,
        string? entidade = null,
        Guid? entidadeId = null,
        string? detalhesJson = null,
        string? ipAddress = null)
    {
        return new AuditLog
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = userId,
            Acao = acao,
            Entidade = entidade,
            EntidadeId = entidadeId,
            DetalhesJson = detalhesJson,
            IpAddress = ipAddress,
            DataHora = DateTime.UtcNow
        };
    }
}