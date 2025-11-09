using LevverRH.Domain.Enums;
using LevverRH.Domain.Events;
using LevverRH.Domain.Exceptions;

namespace LevverRH.Domain.Entities;

public class Tenant
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; } = null!;
    public string Cnpj { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string? Telefone { get; private set; }
    public string? Endereco { get; private set; }
    public TenantStatus Status { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime DataAtualizacao { get; private set; }
    
    // Azure AD / SSO Integration
    public string? Dominio { get; private set; }  // Ex: "levver.ai"
    public string? TenantIdMicrosoft { get; private set; }  // tid do token Azure AD

    private readonly List<object> _domainEvents = new();
    public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

    // EF Constructor
    private Tenant() { }

    public Tenant(string nome, string cnpj, string email, string? telefone = null, string? endereco = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("Nome é obrigatório.");

        if (string.IsNullOrWhiteSpace(cnpj))
            throw new DomainException("CNPJ é obrigatório.");

        if (!ValidarCnpj(cnpj))
            throw new DomainException("CNPJ inválido.");

        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email é obrigatório.");

        if (!ValidarEmail(email))
            throw new DomainException("Email inválido.");

        Id = Guid.NewGuid();
        Nome = nome;
        Cnpj = cnpj;
        Email = email;
        Telefone = telefone;
        Endereco = endereco;
        Status = TenantStatus.Ativo;
        DataCriacao = DateTime.UtcNow;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Ativar()
    {
        Status = TenantStatus.Ativo;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Desativar()
    {
        Status = TenantStatus.Inativo;
        DataAtualizacao = DateTime.UtcNow;

        _domainEvents.Add(new TenantDesativadoEvent(Id));
    }

    public void Suspender()
    {
        Status = TenantStatus.Suspenso;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void AtualizarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("Nome é obrigatório.");
        
        Nome = nome;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void AtualizarCnpj(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            throw new DomainException("CNPJ é obrigatório.");
        
        if (!ValidarCnpj(cnpj))
            throw new DomainException("CNPJ inválido.");
        
        Cnpj = cnpj;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void AtualizarEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email é obrigatório.");
        
        if (!ValidarEmail(email))
            throw new DomainException("Email inválido.");
        
        Email = email;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void AtualizarTelefone(string? telefone)
    {
        Telefone = telefone;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void AtualizarEndereco(string? endereco)
    {
        Endereco = endereco;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void AtualizarDominio(string? dominio)
    {
        if (!string.IsNullOrWhiteSpace(dominio))
        {
            // Validar formato de domínio
            if (!dominio.Contains('.'))
                throw new DomainException("Domínio inválido.");
            
            Dominio = dominio.ToLowerInvariant();
        }
        else
        {
            Dominio = null;
        }
        
        DataAtualizacao = DateTime.UtcNow;
    }

    public void AtualizarTenantIdMicrosoft(string? tenantIdMicrosoft)
    {
        TenantIdMicrosoft = tenantIdMicrosoft;
        DataAtualizacao = DateTime.UtcNow;
    }

    // Factory Method para criação via SSO (primeiro login do domínio)
    public static Tenant CriarPendenteSetupViaSSO(
        string dominio, 
        string emailPrimeiroUsuario, 
        string tenantIdMicrosoft)
    {
        if (string.IsNullOrWhiteSpace(dominio))
            throw new DomainException("Domínio é obrigatório.");

        if (!dominio.Contains('.'))
            throw new DomainException("Domínio inválido.");

        if (string.IsNullOrWhiteSpace(emailPrimeiroUsuario))
            throw new DomainException("Email é obrigatório.");

        return new Tenant
        {
            Id = Guid.NewGuid(),
            Nome = dominio, // Temporário - será atualizado no setup
            Cnpj = string.Empty, // Será preenchido no setup
            Email = emailPrimeiroUsuario,
            Telefone = null,
            Endereco = null,
            Dominio = dominio.ToLowerInvariant(),
            TenantIdMicrosoft = tenantIdMicrosoft,
            Status = TenantStatus.PendenteSetup,
            DataCriacao = DateTime.UtcNow,
            DataAtualizacao = DateTime.UtcNow
        };
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    private static bool ValidarCnpj(string cnpj)
    {
        var apenasNumeros = new string(cnpj.Where(char.IsDigit).ToArray());
        return apenasNumeros.Length == 14;
    }

    private static bool ValidarEmail(string email)
    {
        return email.Contains('@') && email.Contains('.');
    }
}