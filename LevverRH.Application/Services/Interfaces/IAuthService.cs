using LevverRH.Application.DTOs.Auth;
using LevverRH.Application.DTOs.Common;

namespace LevverRH.Application.Services.Interfaces;

public interface IAuthService
{
    Task<ResultDTO<LoginResponseDTO>> LoginAsync(LoginRequestDTO dto);
    
    /// <summary>
    /// Registrar nova empresa (Tenant) com usuário administrador
    /// </summary>
    Task<ResultDTO<LoginResponseDTO>> RegisterTenantAsync(RegisterTenantRequestDTO dto);
    
    /// <summary>
    /// Registrar novo usuário em um tenant existente
    /// </summary>
    Task<ResultDTO<LoginResponseDTO>> RegisterUserAsync(RegisterRequestDTO dto);
    
    Task<ResultDTO<LoginResponseDTO>> LoginWithAzureAdAsync(AzureAdLoginRequestDTO dto);
    
    /// <summary>
    /// Redefinir senha (versão simples para testes)
    /// </summary>
    Task<ResultDTO<string>> ResetPasswordAsync(string email, string newPassword);
}