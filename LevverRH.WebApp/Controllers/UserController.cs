using LevverRH.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LevverRH.WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // ? Todos os endpoints precisam de autenticação
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// Obter informações do usuário logado
    /// </summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "Token inválido" });

        var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));

        if (user == null)
            return NotFound(new { message = "Usuário não encontrado" });

        return Ok(new
        {
            id = user.Id,
            nome = user.Nome,
            email = user.Email,
            role = user.Role.ToString(),
            authType = user.AuthType.ToString(),
            ativo = user.Ativo,
            ultimoLogin = user.UltimoLogin,
            tenantId = user.TenantId
        });
    }

    /// <summary>
    /// Listar todos os usuários do tenant (Admin apenas)
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "AdminOnly")] // ? Apenas Admins
    public async Task<IActionResult> GetUsersByTenant()
    {
        var tenantId = User.FindFirst("TenantId")?.Value;

        if (string.IsNullOrEmpty(tenantId))
            return Unauthorized(new { message = "TenantId não encontrado no token" });

        var users = await _userRepository.GetByTenantIdAsync(Guid.Parse(tenantId));

        var result = users.Select(u => new
        {
            id = u.Id,
            nome = u.Nome,
            email = u.Email,
            role = u.Role.ToString(),
            authType = u.AuthType.ToString(),
            ativo = u.Ativo,
            ultimoLogin = u.UltimoLogin
        });

        return Ok(new
        {
            tenantId = tenantId,
            count = result.Count(),
            users = result
        });
    }

    /// <summary>
    /// Obter usuário por ID (Admin ou Recruiter)
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "AdminOrRecruiter")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var tenantId = User.FindFirst("TenantId")?.Value;
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            return NotFound(new { message = "Usuário não encontrado" });

        // Verificar se o usuário pertence ao mesmo tenant
        if (user.TenantId.ToString() != tenantId)
            return Forbid(); // 403 Forbidden

        return Ok(new
        {
            id = user.Id,
            nome = user.Nome,
            email = user.Email,
            role = user.Role.ToString(),
            authType = user.AuthType.ToString(),
            ativo = user.Ativo,
            ultimoLogin = user.UltimoLogin,
            dataCriacao = user.DataCriacao,
            fotoUrl = user.FotoUrl
        });
    }

    /// <summary>
    /// Desativar usuário (Admin apenas)
    /// </summary>
    [HttpPatch("{id:guid}/deactivate")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeactivateUser(Guid id)
    {
        var tenantId = User.FindFirst("TenantId")?.Value;
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (id.ToString() == currentUserId)
            return BadRequest(new { message = "Você não pode desativar sua própria conta" });

        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            return NotFound(new { message = "Usuário não encontrado" });

        if (user.TenantId.ToString() != tenantId)
            return Forbid();

        user.Desativar();
        await _userRepository.UpdateAsync(user);

        return Ok(new
        {
            message = "Usuário desativado com sucesso",
            userId = id
        });
    }

    /// <summary>
    /// Ativar usuário (Admin apenas)
    /// </summary>
    [HttpPatch("{id:guid}/activate")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> ActivateUser(Guid id)
    {
        var tenantId = User.FindFirst("TenantId")?.Value;
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            return NotFound(new { message = "Usuário não encontrado" });

        if (user.TenantId.ToString() != tenantId)
            return Forbid();

        user.Ativar();
        await _userRepository.UpdateAsync(user);

        return Ok(new
        {
            message = "Usuário ativado com sucesso",
            userId = id
        });
    }
}
