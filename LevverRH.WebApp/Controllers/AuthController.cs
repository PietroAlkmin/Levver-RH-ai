using LevverRH.Application.DTOs.Auth;
using LevverRH.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
    /// Registrar novo usuário
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDTO dto)
    {
        var result = await _authService.RegisterAsync(dto);

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
}