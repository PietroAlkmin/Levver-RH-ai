using LevverRH.Domain.Entities.Talents;

namespace LevverRH.Domain.Interfaces.Talents
{
    public interface ICandidateRepository : IRepository<Candidate>
    {
        Task<IEnumerable<Candidate>> GetByTenantIdAsync(Guid tenantId);
        Task<Candidate?> GetByEmailAsync(string email, Guid tenantId);
        Task<IEnumerable<Candidate>> SearchAsync(Guid tenantId, string searchTerm);
    }
}
