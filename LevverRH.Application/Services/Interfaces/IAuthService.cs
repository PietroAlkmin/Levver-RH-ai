using LevverRH.Application.DTOs.Auth;
using LevverRH.Application.DTOs.Common;

namespace LevverRH.Application.Services.Interfaces;

public interface IAuthService
{
    Task<ResultDTO<LoginResponseDTO>> LoginAsync(LoginRequestDTO dto);
    Task<ResultDTO<LoginResponseDTO>> RegisterAsync(RegisterRequestDTO dto);
    Task<ResultDTO<LoginResponseDTO>> LoginWithAzureAdAsync(AzureAdLoginRequestDTO dto);
}