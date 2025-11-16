using LevverRH.Domain.Entities.Talents;
using LevverRH.Domain.Enums.Talents;

namespace LevverRH.Domain.Interfaces.Talents
{
    public interface IJobRepository : IRepository<Job>
    {
        Task<IEnumerable<Job>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<Job>> GetByStatusAsync(Guid tenantId, JobStatus status);
        Task<IEnumerable<Job>> GetActiveJobsAsync(Guid tenantId);
        Task<int> CountActiveJobsByTenantAsync(Guid tenantId);
    }
}
