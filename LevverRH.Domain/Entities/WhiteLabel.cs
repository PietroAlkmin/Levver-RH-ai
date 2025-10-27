using LevverRH.Domain.Exceptions;

namespace LevverRH.Domain.Entities;

public class WhiteLabel
{
    public int Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string? LogoUrl { get; private set; }
    public string PrimaryColor { get; private set; } = null!;
    public string SecondaryColor { get; private set; } = null!;
    public string AccentColor { get; private set; } = null!;
    public string BackgroundColor { get; private set; } = null!;
    public string TextColor { get; private set; } = null!;
    public string BorderColor { get; private set; } = null!;
    public string SystemName { get; private set; } = null!;
    public string? FaviconUrl { get; private set; }
    public string? DominioCustomizado { get; private set; }
    public string? EmailRodape { get; private set; }
    public bool Active { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Navigation
    public virtual Tenant Tenant { get; private set; } = null!;

    // EF Constructor
    private WhiteLabel() { }

    public WhiteLabel(Guid tenantId, string systemName, Tenant tenant)
    {
        if (tenant == null)
            throw new DomainException("Tenant não existe.");

        TenantId = tenantId;
        SystemName = systemName ?? "Talents";
        Tenant = tenant;
        Active = true;

        // Cores padrão
        PrimaryColor = "#0066CC";
        SecondaryColor = "#00AAFF";
        AccentColor = "#FF6B35";
        BackgroundColor = "#FFFFFF";
        TextColor = "#333333";
        BorderColor = "#E0E0E0";

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Ativar()
    {
        Active = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Desativar()
    {
        Active = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AtualizarCores(
        string primaryColor,
        string secondaryColor,
        string accentColor,
        string backgroundColor,
        string textColor,
        string borderColor)
    {
        ValidarCorHex(primaryColor, nameof(primaryColor));
        ValidarCorHex(secondaryColor, nameof(secondaryColor));
        ValidarCorHex(accentColor, nameof(accentColor));
        ValidarCorHex(backgroundColor, nameof(backgroundColor));
        ValidarCorHex(textColor, nameof(textColor));
        ValidarCorHex(borderColor, nameof(borderColor));

        PrimaryColor = primaryColor;
        SecondaryColor = secondaryColor;
        AccentColor = accentColor;
        BackgroundColor = backgroundColor;
        TextColor = textColor;
        BorderColor = borderColor;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AtualizarLogo(string? url)
    {
        LogoUrl = url;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AtualizarFavicon(string? url)
    {
        FaviconUrl = url;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AtualizarDominioCustomizado(string? dominio)
    {
        if (!string.IsNullOrWhiteSpace(dominio) && !ValidarDominio(dominio))
            throw new DomainException("Domínio inválido.");

        DominioCustomizado = dominio;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AtualizarSystemName(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("Nome do sistema é obrigatório.");

        SystemName = nome;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidarCorHex(string cor, string nomeCampo)
    {
        if (string.IsNullOrWhiteSpace(cor))
            throw new DomainException($"{nomeCampo} é obrigatório.");

        if (!cor.StartsWith("#") || (cor.Length != 7 && cor.Length != 4))
            throw new DomainException($"{nomeCampo} deve estar no formato hex (#RRGGBB ou #RGB).");
    }

    private static bool ValidarDominio(string dominio)
    {
        return dominio.Contains('.') && !dominio.Contains(' ');
    }
}