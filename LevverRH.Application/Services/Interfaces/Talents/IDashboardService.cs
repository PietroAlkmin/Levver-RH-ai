using LevverRH.Application.DTOs.Common;
using LevverRH.Application.DTOs.Talents;

namespace LevverRH.Application.Services.Interfaces.Talents
{
    public interface IDashboardService
    {
        Task<ResultDTO<DashboardStatsDTO>> GetStatsAsync(Guid tenantId);
    }
}
