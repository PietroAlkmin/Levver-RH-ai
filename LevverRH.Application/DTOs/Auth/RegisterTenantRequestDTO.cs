namespace LevverRH.Application.DTOs.Auth;

/// <summary>
/// DTO para registro de nova empresa (Tenant) com usuário administrador
/// </summary>
public class RegisterTenantRequestDTO
{
    // Dados da Empresa (Tenant)
    public string NomeEmpresa { get; set; } = null!;
    public string Cnpj { get; set; } = null!;
    public string EmailEmpresa { get; set; } = null!;
    public string? TelefoneEmpresa { get; set; }
    public string? EnderecoEmpresa { get; set; }

    // Dados do Usuário Administrador
    public string NomeAdmin { get; set; } = null!;
    public string EmailAdmin { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
}
