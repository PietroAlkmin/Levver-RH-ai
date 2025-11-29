using LevverRH.Application.DTOs.Talents;
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
    public class JobsController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobsController(IJobService jobService)
        {
            _jobService = jobService;
        }

        private Guid GetTenantId() => Guid.Parse(User.FindFirstValue("TenantId") ?? throw new UnauthorizedAccessException());
        private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException());

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tenantId = GetTenantId();
            var result = await _jobService.GetAllAsync(tenantId);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var tenantId = GetTenantId();
            var result = await _jobService.GetByIdAsync(id, tenantId);
            
            if (!result.Success)
                return NotFound(result);
            
            return Ok(result);
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(JobStatus status)
        {
            var tenantId = GetTenantId();
            var result = await _jobService.GetByStatusAsync(tenantId, status);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateJobDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tenantId = GetTenantId();
            var userId = GetUserId();
            var result = await _jobService.CreateAsync(dto, tenantId, userId);
            
            if (!result.Success)
                return BadRequest(result);
            
            return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateJobDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tenantId = GetTenantId();
            var result = await _jobService.UpdateAsync(id, dto, tenantId);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var tenantId = GetTenantId();
            var result = await _jobService.DeleteAsync(id, tenantId);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] JobStatus newStatus)
        {
            var tenantId = GetTenantId();
            var result = await _jobService.ChangeStatusAsync(id, newStatus, tenantId);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        #region Criação com IA

        /// <summary>
        /// Inicia criação de vaga assistida por IA
        /// </summary>
        [HttpPost("ai/start")]
        public async Task<IActionResult> StartAICreation([FromBody] StartJobCreationDTO dto)
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            var result = await _jobService.StartJobCreationWithAIAsync(dto, tenantId, userId);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        /// <summary>
        /// [DEV] Inicia criação de vaga com IA - SEM AUTENTICAÇÃO (apenas para testes)
        /// </summary>
        [AllowAnonymous]
        [HttpPost("ai/start-dev")]
        public async Task<IActionResult> StartAICreationDev([FromBody] StartJobCreationDTO dto)
        {
            // IDs fixos para teste - substitua pelos IDs reais do seu banco
            var tenantId = Guid.Parse("11111111-1111-1111-1111-111111111111"); // Tenant de teste
            var userId = Guid.Parse("22222222-2222-2222-2222-222222222222");   // User de teste
            
            var result = await _jobService.StartJobCreationWithAIAsync(dto, tenantId, userId);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        /// <summary>
        /// Envia mensagem no chat de criação de vaga com IA
        /// </summary>
        [HttpPost("ai/chat")]
        public async Task<IActionResult> ProcessAIChatMessage([FromBody] JobChatMessageDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tenantId = GetTenantId();
            var result = await _jobService.ProcessAIChatMessageAsync(dto, tenantId);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        /// <summary>
        /// Finaliza criação de vaga e opcionalmente publica
        /// </summary>
        [HttpPost("ai/complete")]
        public async Task<IActionResult> CompleteAICreation([FromBody] CompleteJobCreationDTO dto)
        {
            var tenantId = GetTenantId();
            var result = await _jobService.CompleteJobCreationAsync(dto, tenantId);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        /// <summary>
        /// Obtém detalhes completos de uma vaga (incluindo todos os campos)
        /// </summary>
        [HttpGet("{id}/detail")]
        public async Task<IActionResult> GetDetailById(Guid id)
        {
            var tenantId = GetTenantId();
            var result = await _jobService.GetDetailByIdAsync(id, tenantId);
            
            if (!result.Success)
                return NotFound(result);
            
            return Ok(result);
        }

        /// <summary>
        /// Lista rascunhos do usuário atual
        /// </summary>
        [HttpGet("drafts")]
        public async Task<IActionResult> GetMyDrafts()
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            var result = await _jobService.GetDraftsByUserAsync(userId, tenantId);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        #endregion
    }
}
