namespace LevverRH.Application.DTOs.Auth;

/// <summary>
/// DTO para completar o setup de um Tenant criado via SSO
/// (não inclui dados do admin pois o usuário já existe)
/// </summary>
public class CompleteTenantSetupDTO
{
    // Dados da Empresa (Tenant)
    public string NomeEmpresa { get; set; } = null!;
    public string Cnpj { get; set; } = null!;
    public string EmailEmpresa { get; set; } = null!;
    public string? TelefoneEmpresa { get; set; }
    public string? EnderecoEmpresa { get; set; }
}
