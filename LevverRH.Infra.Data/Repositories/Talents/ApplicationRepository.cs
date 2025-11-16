using LevverRH.Domain.Entities.Talents;
using LevverRH.Domain.Enums.Talents;
using LevverRH.Domain.Interfaces.Talents;
using LevverRH.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace LevverRH.Infra.Data.Repositories.Talents
{
    public class ApplicationRepository : Repository<Application>, IApplicationRepository
    {
        public ApplicationRepository(LevverDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Application>> GetByJobIdAsync(Guid jobId)
        {
            return await _context.Set<Application>()
                .Include(a => a.Candidate)
                .Where(a => a.JobId == jobId)
                .OrderByDescending(a => a.ScoreGeral)
                .ThenByDescending(a => a.DataInscricao)
                .ToListAsync();
        }

        public async Task<IEnumerable<Application>> GetByCandidateIdAsync(Guid candidateId)
        {
            return await _context.Set<Application>()
                .Include(a => a.Job)
                .Where(a => a.CandidateId == candidateId)
                .OrderByDescending(a => a.DataInscricao)
                .ToListAsync();
        }

        public async Task<IEnumerable<Application>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _context.Set<Application>()
                .Include(a => a.Job)
                .Include(a => a.Candidate)
                .Where(a => a.TenantId == tenantId)
                .OrderByDescending(a => a.DataInscricao)
                .ToListAsync();
        }

        public async Task<IEnumerable<Application>> GetByStatusAsync(Guid tenantId, ApplicationStatus status)
        {
            return await _context.Set<Application>()
                .Include(a => a.Job)
                .Include(a => a.Candidate)
                .Where(a => a.TenantId == tenantId && a.Status == status)
                .OrderByDescending(a => a.DataInscricao)
                .ToListAsync();
        }

        public async Task<Application?> GetByJobAndCandidateAsync(Guid jobId, Guid candidateId)
        {
            return await _context.Set<Application>()
                .Include(a => a.Job)
                .Include(a => a.Candidate)
                .FirstOrDefaultAsync(a => a.JobId == jobId && a.CandidateId == candidateId);
        }
    }
}
