using LevverRH.Application.DTOs.Common;
using LevverRH.Application.DTOs.Talents;

namespace LevverRH.Application.Services.Interfaces.Talents
{
    public interface ICandidateService
    {
        Task<ResultDTO<IEnumerable<CandidateDTO>>> GetAllAsync(Guid tenantId);
        Task<ResultDTO<CandidateDTO>> GetByIdAsync(Guid id, Guid tenantId);
        Task<ResultDTO<CandidateDTO>> CreateAsync(CreateCandidateDTO dto, Guid tenantId);
        Task<ResultDTO<IEnumerable<CandidateDTO>>> SearchAsync(Guid tenantId, string searchTerm);
    }
}
