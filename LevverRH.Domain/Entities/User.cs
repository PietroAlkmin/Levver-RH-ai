using BCrypt.Net;
using LevverRH.Domain.Enums;
using LevverRH.Domain.Events;
using LevverRH.Domain.Exceptions;

namespace LevverRH.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string Email { get; private set; } = null!;
    public string Nome { get; private set; } = null!;
    public UserRole Role { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime? UltimoLogin { get; private set; }
    public string? FotoUrl { get; private set; }

    // Autenticação híbrida
    public AuthType AuthType { get; private set; }
    public string? AzureAdId { get; private set; }
    public string? PasswordHash { get; private set; }

    // Navigation
    public virtual Tenant Tenant { get; private set; } = null!;

    private readonly List<object> _domainEvents = new();
    public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

    // EF Constructor
    private User() { }

    // Factory Method para Azure AD
    public static User CriarComAzureAd(
        Guid tenantId,
        string azureAdId,
        string email,
        string nome,
        UserRole role,
        Tenant tenant)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email é obrigatório.");

        if (!ValidarEmail(email))
            throw new DomainException("Email inválido.");

        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("Nome é obrigatório.");

        if (string.IsNullOrWhiteSpace(azureAdId))
            throw new DomainException("Azure AD ID é obrigatório para autenticação Azure AD.");

        if (tenant == null)
            throw new DomainException("Tenant não existe.");

        if (tenant.Status != TenantStatus.Ativo)
            throw new DomainException("Tenant não está ativo.");

        return new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Email = email.ToLowerInvariant(), // Normalizar email
            Nome = nome,
            Role = role,
            AuthType = AuthType.AzureAd,
            AzureAdId = azureAdId,
            PasswordHash = null,
            Ativo = true,
            DataCriacao = DateTime.UtcNow,
            Tenant = tenant
        };
    }

    // Factory Method para Local (email/senha)
    public static User CriarComSenha(
        Guid tenantId,
        string email,
        string nome,
        string passwordHash,
        UserRole role,
        Tenant tenant)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email é obrigatório.");

        if (!ValidarEmail(email))
            throw new DomainException("Email inválido.");

        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("Nome é obrigatório.");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Hash de senha é obrigatório para autenticação local.");

        if (tenant == null)
            throw new DomainException("Tenant não existe.");

        if (tenant.Status != TenantStatus.Ativo)
            throw new DomainException("Tenant não está ativo.");

        return new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Email = email.ToLowerInvariant(), // Normalizar email
            Nome = nome,
            Role = role,
            AuthType = AuthType.Local,
            AzureAdId = null,
            PasswordHash = passwordHash,
            Ativo = true,
            DataCriacao = DateTime.UtcNow,
            Tenant = tenant
        };
    }

    public void Ativar()
    {
        Ativo = true;
    }

    public void Desativar()
    {
        Ativo = false;
    }

    public void RegistrarLogin()
    {
        UltimoLogin = DateTime.UtcNow;
    }

    public void AtualizarRole(UserRole novaRole, User userQueAltera)
    {
        if (userQueAltera.Role != UserRole.Admin)
            throw new UnauthorizedException("Apenas Admin pode alterar roles.");

        if (Id == userQueAltera.Id)
            throw new DomainException("Usuário não pode alterar sua própria role.");

        var roleAnterior = Role;
        Role = novaRole;

        _domainEvents.Add(new UserRoleChangedEvent(Id, roleAnterior, novaRole, userQueAltera.Id));
    }

    public void AtualizarFoto(string? url)
    {
        FotoUrl = url;
    }

    public void ValidarAcesso(Guid tenantId)
    {
        if (TenantId != tenantId)
            throw new UnauthorizedException("Usuário não pertence a este tenant.");

        if (!Ativo)
            throw new UnauthorizedException("Usuário está inativo.");

        if (Tenant?.Status != TenantStatus.Ativo)
            throw new TenantInativoException(TenantId);
    }

    // Validar senha (apenas para AuthType.Local)
    public bool ValidatePassword(string password)
    {
        if (AuthType != AuthType.Local)
            throw new DomainException("Usuário não utiliza autenticação local.");

        if (string.IsNullOrWhiteSpace(PasswordHash))
            throw new DomainException("Usuário não possui senha cadastrada.");

        return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
    }

    // Atualizar senha (apenas para AuthType.Local)
    public void AtualizarSenha(string novaSenhaHash)
    {
        if (AuthType != AuthType.Local)
            throw new DomainException("Usuário não utiliza autenticação local.");

        if (string.IsNullOrWhiteSpace(novaSenhaHash))
            throw new DomainException("Hash de senha é obrigatório.");

        PasswordHash = novaSenhaHash;
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    private static bool ValidarEmail(string email)
    {
        return email.Contains('@') && email.Contains('.');
    }
}