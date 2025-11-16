using LevverRH.Domain.Entities.Talents;
using LevverRH.Domain.Enums.Talents;
using LevverRH.Domain.Interfaces.Talents;
using LevverRH.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace LevverRH.Infra.Data.Repositories.Talents
{
    public class JobRepository : Repository<Job>, IJobRepository
    {
        public JobRepository(LevverDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Job>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _context.Set<Job>()
                .Where(j => j.TenantId == tenantId)
                .OrderByDescending(j => j.DataCriacao)
                .ToListAsync();
        }

        public async Task<IEnumerable<Job>> GetByStatusAsync(Guid tenantId, JobStatus status)
        {
            return await _context.Set<Job>()
                .Where(j => j.TenantId == tenantId && j.Status == status)
                .OrderByDescending(j => j.DataCriacao)
                .ToListAsync();
        }

        public async Task<IEnumerable<Job>> GetActiveJobsAsync(Guid tenantId)
        {
            return await _context.Set<Job>()
                .Where(j => j.TenantId == tenantId && j.Status == JobStatus.Aberta)
                .OrderByDescending(j => j.DataCriacao)
                .ToListAsync();
        }

        public async Task<int> CountActiveJobsByTenantAsync(Guid tenantId)
        {
            return await _context.Set<Job>()
                .CountAsync(j => j.TenantId == tenantId && j.Status == JobStatus.Aberta);
        }
    }
}
