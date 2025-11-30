using LevverRH.Application.Services.Interfaces.Talents;
using LevverRH.Domain.Enums.Talents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LevverRH.WebApp.Controllers.Talents
{
    [Authorize]
    [ApiController]
    [Route("api/talents/[controller]")]
    public class ApplicationsController : ControllerBase
    {
        private readonly IApplicationService _applicationService;

        public ApplicationsController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        private Guid GetTenantId() => Guid.Parse(User.FindFirstValue("TenantId") ?? throw new UnauthorizedAccessException());
        private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException());

        [HttpGet("job/{jobId}")]
        public async Task<IActionResult> GetByJobId(Guid jobId)
        {
            var tenantId = GetTenantId();
            var result = await _applicationService.GetByJobIdAsync(jobId, tenantId);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var tenantId = GetTenantId();
            var result = await _applicationService.GetByIdAsync(id, tenantId);
            
            if (!result.Success)
                return NotFound(result);
            
            return Ok(result);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ApplicationStatus newStatus)
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            var result = await _applicationService.ChangeStatusAsync(id, newStatus, tenantId, userId);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpPatch("{id}/favorito")]
        public async Task<IActionResult> ToggleFavorito(Guid id)
        {
            var tenantId = GetTenantId();
            var result = await _applicationService.ToggleFavoritoAsync(id, tenantId);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpPost("{id}/analyze")]
        public async Task<IActionResult> AnalyzeWithAI(Guid id)
        {
            var tenantId = GetTenantId();
            var result = await _applicationService.AnalyzeCandidateWithAIAsync(id, tenantId);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }
    }
}
