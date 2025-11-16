using LevverRH.Domain.Entities.Talents;
using LevverRH.Domain.Enums.Talents;

namespace LevverRH.Domain.Interfaces.Talents
{
    public interface IApplicationRepository : IRepository<Application>
    {
        Task<IEnumerable<Application>> GetByJobIdAsync(Guid jobId);
        Task<IEnumerable<Application>> GetByCandidateIdAsync(Guid candidateId);
        Task<IEnumerable<Application>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<Application>> GetByStatusAsync(Guid tenantId, ApplicationStatus status);
        Task<Application?> GetByJobAndCandidateAsync(Guid jobId, Guid candidateId);
    }
}
