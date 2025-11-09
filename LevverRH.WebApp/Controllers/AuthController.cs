using LevverRH.Application.DTOs.Auth;
using LevverRH.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LevverRH.WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Login com email e senha
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO dto)
    {
        var result = await _authService.LoginAsync(dto);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Registrar nova empresa (Tenant) com usuário administrador - Onboarding
    /// </summary>
    [HttpPost("register/tenant")]
    public async Task<IActionResult> RegisterTenant([FromBody] RegisterTenantRequestDTO dto)
    {
        var result = await _authService.RegisterTenantAsync(dto);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Registrar novo usuário em um tenant existente
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDTO dto)
    {
        var result = await _authService.RegisterUserAsync(dto);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Login com Azure AD (SSO)
    /// </summary>
    [HttpPost("login/azure")]
    public async Task<IActionResult> LoginAzureAd([FromBody] AzureAdLoginRequestDTO dto)
    {
        var result = await _authService.LoginWithAzureAdAsync(dto);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Completar setup de tenant criado via SSO (após primeiro login Azure AD)
    /// </summary>
    [HttpPost("complete-tenant-setup")]
    [Authorize] // Requer autenticação
    public async Task<IActionResult> CompleteTenantSetup([FromBody] CompleteTenantSetupDTO dto)
    {
        Console.WriteLine("🔹 Endpoint /complete-tenant-setup chamado!");
        Console.WriteLine($"🔹 CNPJ: {dto.Cnpj}, Nome: {dto.NomeEmpresa}");
        
        // Pegar ID do usuário logado do JWT
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? User.FindFirst("sub")?.Value;

        Console.WriteLine($"🔹 UserID do token: {userIdClaim}");
        
        if (string.IsNullOrEmpty(userIdClaim))
        {
            Console.WriteLine("❌ Token inválido - UserID não encontrado");
            return Unauthorized("Token inválido");
        }

        var userId = Guid.Parse(userIdClaim);

        var result = await _authService.CompleteTenantSetupAsync(userId, dto);

        if (!result.Success)
        {
            Console.WriteLine($"❌ Erro no setup: {result.Message}");
            return BadRequest(result);
        }

        Console.WriteLine("✅ Setup concluído com sucesso!");
        return Ok(result);
    }
}