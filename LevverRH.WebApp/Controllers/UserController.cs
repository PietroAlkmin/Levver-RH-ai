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
    private readonly IStorageService _storageService;

    public UserController(IUserRepository userRepository, IStorageService storageService)
    {
        _userRepository = userRepository;
        _storageService = storageService;
    }

    /// <summary>
    /// Obter informa��es do usu�rio logado
    /// </summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "Token inv�lido" });

        var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));

        if (user == null)
            return NotFound(new { message = "Usu�rio n�o encontrado" });

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
    /// Listar todos os usu�rios do tenant (Admin apenas)
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "AdminOnly")] // ? Apenas Admins
    public async Task<IActionResult> GetUsersByTenant()
    {
        var tenantId = User.FindFirst("TenantId")?.Value;

        if (string.IsNullOrEmpty(tenantId))
            return Unauthorized(new { message = "TenantId n�o encontrado no token" });

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
    /// Obter usu�rio por ID (Admin ou Recruiter)
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "AdminOrRecruiter")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var tenantId = User.FindFirst("TenantId")?.Value;
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            return NotFound(new { message = "Usu�rio n�o encontrado" });

        // Verificar se o usu�rio pertence ao mesmo tenant
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
    /// Desativar usu�rio (Admin apenas)
    /// </summary>
    [HttpPatch("{id:guid}/deactivate")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeactivateUser(Guid id)
    {
        var tenantId = User.FindFirst("TenantId")?.Value;
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (id.ToString() == currentUserId)
            return BadRequest(new { message = "Voc� n�o pode desativar sua pr�pria conta" });

        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            return NotFound(new { message = "Usu�rio n�o encontrado" });

        if (user.TenantId.ToString() != tenantId)
            return Forbid();

        user.Desativar();
        await _userRepository.UpdateAsync(user);

        return Ok(new
        {
            message = "Usu�rio desativado com sucesso",
            userId = id
        });
    }

    /// <summary>
    /// Ativar usu�rio (Admin apenas)
    /// </summary>
    [HttpPatch("{id:guid}/activate")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> ActivateUser(Guid id)
    {
        var tenantId = User.FindFirst("TenantId")?.Value;
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            return NotFound(new { message = "Usu�rio n�o encontrado" });

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

    /// <summary>
    /// Upload de foto de perfil do usuário logado
    /// </summary>
    [HttpPost("photo")]
    public async Task<IActionResult> UploadProfilePhoto(IFormFile file)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "Token inválido" });

        if (file == null || file.Length == 0)
            return BadRequest(new { message = "Nenhum arquivo foi enviado" });

        // Validar tipo de arquivo
        var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
        if (!allowedTypes.Contains(file.ContentType.ToLower()))
            return BadRequest(new { message = "Tipo de arquivo não permitido. Use JPG, PNG ou GIF" });

        // Validar tamanho (máx 5MB)
        if (file.Length > 5 * 1024 * 1024)
            return BadRequest(new { message = "O arquivo deve ter no máximo 5MB" });

        try
        {
            var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));

            if (user == null)
                return NotFound(new { message = "Usuário não encontrado" });

            // Deletar foto antiga se existir
            if (!string.IsNullOrEmpty(user.FotoUrl))
            {
                await _storageService.DeleteFileAsync(user.FotoUrl);
            }

            // Upload da nova foto
            using var stream = file.OpenReadStream();
            var photoUrl = await _storageService.UploadProfilePhotoAsync(
                Guid.Parse(userId),
                stream,
                file.FileName,
                file.ContentType
            );

            // Atualizar usuário
            user.AtualizarFoto(photoUrl);
            await _userRepository.UpdateAsync(user);

            return Ok(new
            {
                message = "Foto atualizada com sucesso",
                fotoUrl = photoUrl
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao fazer upload da foto", error = ex.Message });
        }
    }

    /// <summary>
    /// Remover foto de perfil do usuário logado
    /// </summary>
    [HttpDelete("photo")]
    public async Task<IActionResult> DeleteProfilePhoto()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "Token inválido" });

        try
        {
            var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));

            if (user == null)
                return NotFound(new { message = "Usuário não encontrado" });

            if (string.IsNullOrEmpty(user.FotoUrl))
                return BadRequest(new { message = "Usuário não possui foto de perfil" });

            // Deletar foto do blob
            await _storageService.DeleteFileAsync(user.FotoUrl);

            // Atualizar usuário
            user.AtualizarFoto(null);
            await _userRepository.UpdateAsync(user);

            return Ok(new { message = "Foto removida com sucesso" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao remover foto", error = ex.Message });
        }
    }
}
