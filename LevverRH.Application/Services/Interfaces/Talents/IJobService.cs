using LevverRH.Application.DTOs.Common;
using LevverRH.Application.DTOs.Talents;
using LevverRH.Domain.Enums.Talents;

namespace LevverRH.Application.Services.Interfaces.Talents
{
    public interface IJobService
    {
        Task<ResultDTO<IEnumerable<JobDTO>>> GetAllAsync(Guid tenantId);
        Task<ResultDTO<JobDTO>> GetByIdAsync(Guid id, Guid tenantId);
        Task<ResultDTO<JobDTO>> CreateAsync(CreateJobDTO dto, Guid tenantId, Guid userId);
        Task<ResultDTO<JobDTO>> UpdateAsync(Guid id, UpdateJobDTO dto, Guid tenantId);
        Task<ResultDTO<bool>> DeleteAsync(Guid id, Guid tenantId);
        Task<ResultDTO<IEnumerable<JobDTO>>> GetByStatusAsync(Guid tenantId, JobStatus status);
        Task<ResultDTO<bool>> ChangeStatusAsync(Guid id, JobStatus newStatus, Guid tenantId);
    }
}
