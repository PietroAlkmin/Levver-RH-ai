using LevverRH.Domain.Entities.Talents;
using LevverRH.Domain.Interfaces.Talents;
using LevverRH.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace LevverRH.Infra.Data.Repositories.Talents
{
    public class CandidateRepository : Repository<Candidate>, ICandidateRepository
    {
        public CandidateRepository(LevverDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Candidate>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _context.Set<Candidate>()
                .Where(c => c.TenantId == tenantId)
                .OrderByDescending(c => c.DataCadastro)
                .ToListAsync();
        }

        public async Task<Candidate?> GetByEmailAsync(Guid tenantId, string email)
        {
            return await _context.Set<Candidate>()
                .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.Email == email);
        }

        public async Task<IEnumerable<Candidate>> SearchAsync(Guid tenantId, string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();
            
            return await _context.Set<Candidate>()
                .Where(c => c.TenantId == tenantId && 
                    (c.Nome.ToLower().Contains(lowerSearchTerm) || 
                     c.Email.ToLower().Contains(lowerSearchTerm)))
                .OrderByDescending(c => c.DataCadastro)
                .ToListAsync();
        }
    }
}
