using LevverRH.Domain.Exceptions;

namespace LevverRH.Domain.Entities;

public class TenantProduct
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid ProductId { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime DataAtivacao { get; private set; }
    public DateTime? DataDesativacao { get; private set; }
    public string? ConfiguracaoJson { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime DataAtualizacao { get; private set; }

    // Navigation Properties
    public virtual Tenant Tenant { get; private set; } = null!;
    public virtual ProductCatalog Product { get; private set; } = null!;

    // EF Constructor
    private TenantProduct() { }

    public TenantProduct(Guid tenantId, Guid productId, string? configuracaoJson = null)
    {
        if (tenantId == Guid.Empty)
            throw new DomainException("TenantId é obrigatório.");

        if (productId == Guid.Empty)
            throw new DomainException("ProductId é obrigatório.");

        Id = Guid.NewGuid();
        TenantId = tenantId;
        ProductId = productId;
        Ativo = true;
        DataAtivacao = DateTime.UtcNow;
        ConfiguracaoJson = configuracaoJson;
        DataCriacao = DateTime.UtcNow;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Ativar()
    {
        if (Ativo)
            throw new DomainException("Produto já está ativo para este tenant.");

        Ativo = true;
        DataAtivacao = DateTime.UtcNow;
        DataDesativacao = null;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Desativar()
    {
        if (!Ativo)
            throw new DomainException("Produto já está desativado para este tenant.");

        Ativo = false;
        DataDesativacao = DateTime.UtcNow;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void AtualizarConfiguracao(string? configuracaoJson)
    {
        ConfiguracaoJson = configuracaoJson;
        DataAtualizacao = DateTime.UtcNow;
    }
}
