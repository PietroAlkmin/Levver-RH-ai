using LevverRH.Application.DTOs.Common;
using LevverRH.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LevverRH.WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WhiteLabelController : ControllerBase
{
    private readonly IWhiteLabelRepository _whiteLabelRepository;
    private readonly IStorageService _storageService;
    private readonly ILogger<WhiteLabelController> _logger;

    public WhiteLabelController(
        IWhiteLabelRepository whiteLabelRepository,
        IStorageService storageService,
        ILogger<WhiteLabelController> logger)
    {
        _whiteLabelRepository = whiteLabelRepository;
        _storageService = storageService;
        _logger = logger;
    }

    [HttpPost("upload-logo")]
    public async Task<ActionResult<ResultDTO<string>>> UploadLogo(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest(ResultDTO<string>.FailureResult("Arquivo inválido"));

            var tenantIdClaim = User.FindFirst("TenantId")?.Value;
            if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out var tenantId))
                return Unauthorized(ResultDTO<string>.FailureResult("Tenant não identificado"));

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".svg", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(extension))
                return BadRequest(ResultDTO<string>.FailureResult("Formato de arquivo não suportado. Use: jpg, png, svg ou webp"));

            if (file.Length > 2 * 1024 * 1024) // 2MB
                return BadRequest(ResultDTO<string>.FailureResult("Arquivo muito grande. Tamanho máximo: 2MB"));

            var whiteLabel = await _whiteLabelRepository.GetByTenantIdAsync(tenantId);
            if (whiteLabel == null)
                return NotFound(ResultDTO<string>.FailureResult("WhiteLabel não encontrado para este tenant"));

            // Deletar logo antiga se existir
            if (!string.IsNullOrEmpty(whiteLabel.LogoUrl))
            {
                await _storageService.DeleteFileAsync(whiteLabel.LogoUrl);
            }

            // Upload nova logo
            using var stream = file.OpenReadStream();
            var logoUrl = await _storageService.UploadLogoAsync(tenantId, stream, file.FileName, file.ContentType);

            // Atualizar no banco
            whiteLabel.AtualizarLogo(logoUrl);
            await _whiteLabelRepository.UpdateAsync(whiteLabel);

            _logger.LogInformation("Logo atualizada para tenant {TenantId}: {LogoUrl}", tenantId, logoUrl);

            return Ok(ResultDTO<string>.SuccessResult(logoUrl, "Logo atualizada com sucesso"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao fazer upload da logo");
            return StatusCode(500, ResultDTO<string>.FailureResult("Erro ao fazer upload da logo"));
        }
    }

    [HttpPost("upload-favicon")]
    public async Task<ActionResult<ResultDTO<string>>> UploadFavicon(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest(ResultDTO<string>.FailureResult("Arquivo inválido"));

            var tenantIdClaim = User.FindFirst("TenantId")?.Value;
            if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out var tenantId))
                return Unauthorized(ResultDTO<string>.FailureResult("Tenant não identificado"));

            var allowedExtensions = new[] { ".ico", ".png", ".svg" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(extension))
                return BadRequest(ResultDTO<string>.FailureResult("Formato de arquivo não suportado. Use: ico, png ou svg"));

            if (file.Length > 512 * 1024) // 512KB
                return BadRequest(ResultDTO<string>.FailureResult("Arquivo muito grande. Tamanho máximo: 512KB"));

            var whiteLabel = await _whiteLabelRepository.GetByTenantIdAsync(tenantId);
            if (whiteLabel == null)
                return NotFound(ResultDTO<string>.FailureResult("WhiteLabel não encontrado para este tenant"));

            // Deletar favicon antigo se existir
            if (!string.IsNullOrEmpty(whiteLabel.FaviconUrl))
            {
                await _storageService.DeleteFileAsync(whiteLabel.FaviconUrl);
            }

            // Upload novo favicon
            using var stream = file.OpenReadStream();
            var faviconUrl = await _storageService.UploadFaviconAsync(tenantId, stream, file.FileName, file.ContentType);

            // Atualizar no banco
            whiteLabel.AtualizarFavicon(faviconUrl);
            await _whiteLabelRepository.UpdateAsync(whiteLabel);

            _logger.LogInformation("Favicon atualizado para tenant {TenantId}: {FaviconUrl}", tenantId, faviconUrl);

            return Ok(ResultDTO<string>.SuccessResult(faviconUrl, "Favicon atualizado com sucesso"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao fazer upload do favicon");
            return StatusCode(500, ResultDTO<string>.FailureResult("Erro ao fazer upload do favicon"));
        }
    }

    [HttpPut("colors")]
    public async Task<ActionResult<ResultDTO<bool>>> UpdateColors([FromBody] UpdateColorsRequest request)
    {
        try
        {
            var tenantIdClaim = User.FindFirst("TenantId")?.Value;
            if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out var tenantId))
                return Unauthorized(ResultDTO<bool>.FailureResult("Tenant não identificado"));

            var whiteLabel = await _whiteLabelRepository.GetByTenantIdAsync(tenantId);
            if (whiteLabel == null)
                return NotFound(ResultDTO<bool>.FailureResult("WhiteLabel não encontrado para este tenant"));

            whiteLabel.AtualizarCores(
                request.PrimaryColor,
                request.SecondaryColor,
                request.AccentColor,
                request.BackgroundColor,
                request.TextColor,
                request.BorderColor
            );

            await _whiteLabelRepository.UpdateAsync(whiteLabel);

            _logger.LogInformation("Cores atualizadas para tenant {TenantId}", tenantId);

            return Ok(ResultDTO<bool>.SuccessResult(true, "Cores atualizadas com sucesso"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar cores");
            return StatusCode(500, ResultDTO<bool>.FailureResult(ex.Message));
        }
    }
}

public record UpdateColorsRequest(
    string PrimaryColor,
    string SecondaryColor,
    string AccentColor,
    string BackgroundColor,
    string TextColor,
    string BorderColor
);
