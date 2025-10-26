using LevverRH.Domain.Enums;
using LevverRH.Domain.Events;
using LevverRH.Domain.Exceptions;

namespace LevverRH.Domain.Entities;

public class TenantSubscription
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid ProductCatalogId { get; private set; }
    public decimal ValorBaseContratado { get; private set; }
    public string? ConfigJsonContratado { get; private set; }
    public DateTime DataInicio { get; private set; }
    public DateTime? DataFim { get; private set; }
    public SubscriptionStatus Status { get; private set; }
    public string? ContratoNumero { get; private set; }
    public string? Observacoes { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime DataAtualizacao { get; private set; }

    // Navigation
    public virtual Tenant Tenant { get; private set; } = null!;
    public virtual ProductCatalog ProductCatalog { get; private set; } = null!;

    private readonly List<object> _domainEvents = new();
    public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

    // EF Constructor
    private TenantSubscription() { }

    public TenantSubscription(
        Guid tenantId,
        Guid productCatalogId,
        decimal valorBase,
        DateTime dataInicio,
        Tenant tenant,
        ProductCatalog product,
        DateTime? dataFim = null)
    {
        if (valorBase < 0)
            throw new DomainException("Valor deve ser maior ou igual a zero.");

        if (dataFim.HasValue && dataFim.Value <= dataInicio)
            throw new DomainException("Data fim deve ser posterior à data início.");

        if (tenant == null)
            throw new DomainException("Tenant não existe.");

        if (product == null)
            throw new DomainException("Produto não existe.");

        Id = Guid.NewGuid();
        TenantId = tenantId;
        ProductCatalogId = productCatalogId;
        ValorBaseContratado = valorBase;
        DataInicio = dataInicio;
        DataFim = dataFim;
        Status = SubscriptionStatus.Ativo;
        DataCriacao = DateTime.UtcNow;
        DataAtualizacao = DateTime.UtcNow;
        Tenant = tenant;
        ProductCatalog = product;
    }

    public void Ativar()
    {
        Status = SubscriptionStatus.Ativo;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Cancelar()
    {
        Status = SubscriptionStatus.Cancelado;
        DataFim = DateTime.UtcNow;
        DataAtualizacao = DateTime.UtcNow;

        _domainEvents.Add(new SubscriptionCanceledEvent(Id, TenantId));
    }

    public void Suspender()
    {
        Status = SubscriptionStatus.Suspenso;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Concluir()
    {
        Status = SubscriptionStatus.Concluido;
        DataFim = DateTime.UtcNow;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Reativar()
    {
        Status = SubscriptionStatus.Ativo;
        DataFim = null;
        DataAtualizacao = DateTime.UtcNow;
    }

    public bool EstaVigente()
    {
        if (Status != SubscriptionStatus.Ativo)
            return false;

        var hoje = DateTime.UtcNow.Date;
        var dentroDataInicio = hoje >= DataInicio.Date;
        var dentroDataFim = !DataFim.HasValue || hoje <= DataFim.Value.Date;

        return dentroDataInicio && dentroDataFim;
    }

    public void AtualizarValor(decimal novoValor)
    {
        if (novoValor < 0)
            throw new DomainException("Valor deve ser maior ou igual a zero.");

        ValorBaseContratado = novoValor;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void AtualizarConfiguracao(string? configJson)
    {
        ConfigJsonContratado = configJson;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void RenovarContrato(DateTime novaDataFim)
    {
        if (novaDataFim <= DataInicio)
            throw new DomainException("Data fim deve ser posterior à data início.");

        DataFim = novaDataFim;
        DataAtualizacao = DateTime.UtcNow;
    }

    public int? DiasRestantes()
    {
        if (!DataFim.HasValue)
            return null;

        var dias = (DataFim.Value.Date - DateTime.UtcNow.Date).Days;
        return dias > 0 ? dias : 0;
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}