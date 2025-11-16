using LevverRH.Application.DTOs.Talents;
using LevverRH.Application.Services.Interfaces.Talents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LevverRH.WebApp.Controllers.Talents
{
    [Authorize]
    [ApiController]
    [Route("api/talents/[controller]")]
    public class CandidatesController : ControllerBase
    {
        private readonly ICandidateService _candidateService;

        public CandidatesController(ICandidateService candidateService)
        {
            _candidateService = candidateService;
        }

        private Guid GetTenantId() => Guid.Parse(User.FindFirstValue("TenantId") ?? throw new UnauthorizedAccessException());

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tenantId = GetTenantId();
            var result = await _candidateService.GetAllAsync(tenantId);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var tenantId = GetTenantId();
            var result = await _candidateService.GetByIdAsync(id, tenantId);
            
            if (!result.Success)
                return NotFound(result);
            
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string searchTerm)
        {
            var tenantId = GetTenantId();
            var result = await _candidateService.SearchAsync(tenantId, searchTerm);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCandidateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tenantId = GetTenantId();
            var result = await _candidateService.CreateAsync(dto, tenantId);
            
            if (!result.Success)
                return BadRequest(result);
            
            return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
        }
    }
}
