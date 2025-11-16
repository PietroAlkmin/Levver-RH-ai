using LevverRH.Application.Services.Interfaces.Talents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LevverRH.WebApp.Controllers.Talents
{
    [Authorize]
    [ApiController]
    [Route("api/talents/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        private Guid GetTenantId() => Guid.Parse(User.FindFirstValue("TenantId") ?? throw new UnauthorizedAccessException());

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var tenantId = GetTenantId();
            var result = await _dashboardService.GetStatsAsync(tenantId);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }
    }
}
