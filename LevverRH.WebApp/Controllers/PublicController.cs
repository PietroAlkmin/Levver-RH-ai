using LevverRH.Application.DTOs.Public;
using LevverRH.Application.Services.Interfaces.Talents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LevverRH.WebApp.Controllers
{
    /// <summary>
    /// Controller público (sem autenticação) para candidaturas
    /// </summary>
    [AllowAnonymous]
    [ApiController]
    [Route("api/public")]
    public class PublicController : ControllerBase
    {
        private readonly IJobService _jobService;
        private readonly IApplicationService _applicationService;

        public PublicController(
            IJobService jobService,
            IApplicationService applicationService)
        {
            _jobService = jobService;
            _applicationService = applicationService;
        }

        /// <summary>
        /// Obtém detalhes públicos de uma vaga (sem autenticação)
        /// </summary>
        [HttpGet("jobs/{id}")]
        public async Task<IActionResult> GetPublicJobDetail(Guid id)
        {
            try
            {
                // Busca vaga sem validar tenant (público)
                var result = await _jobService.GetPublicJobDetailAsync(id);
                
                if (!result.Success)
                    return NotFound(result);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Cria uma candidatura pública (sem autenticação)
        /// </summary>
        [HttpPost("applications")]
        public async Task<IActionResult> CreateApplication([FromForm] CreatePublicApplicationDTO dto, IFormFile? curriculo)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                Stream? curriculoStream = null;
                string? curriculoFileName = null;
                string? curriculoContentType = null;

                if (curriculo != null && curriculo.Length > 0)
                {
                    curriculoStream = curriculo.OpenReadStream();
                    curriculoFileName = curriculo.FileName;
                    curriculoContentType = curriculo.ContentType;
                }

                var result = await _applicationService.CreatePublicApplicationAsync(
                    dto,
                    curriculoStream,
                    curriculoFileName,
                    curriculoContentType
                );

                if (!result.Success)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro em CreateApplication: {ex.Message}");
                return StatusCode(500, new { message = "Erro ao processar candidatura" });
            }
        }
    }
}
