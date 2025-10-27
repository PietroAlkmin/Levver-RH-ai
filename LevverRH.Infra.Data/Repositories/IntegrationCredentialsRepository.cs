using LevverRH.Domain.Entities;
using LevverRH.Domain.Interfaces;
using LevverRH.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace LevverRH.Infra.Data.Repositories;

public class IntegrationCredentialsRepository : Repository<IntegrationCredentials>, IIntegrationCredentialsRepository
{
    public IntegrationCredentialsRepository(LevverDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<IntegrationCredentials>> GetByTenantIdAsync(Guid tenantId)
    {
  return await _dbSet
      .Where(i => i.TenantId == tenantId)
    .ToListAsync();
 }

    public async Task<IntegrationCredentials?> GetByTenantAndPlataformaAsync(Guid tenantId, string plataforma)
    {
        return await _dbSet
 .FirstOrDefaultAsync(i => i.TenantId == tenantId && i.Plataforma == plataforma);
    }

    public async Task<IEnumerable<IntegrationCredentials>> GetTokensExpiradosAsync()
    {
 var agora = DateTime.UtcNow;
     return await _dbSet
      .Where(i => i.ExpiresAt.HasValue && i.ExpiresAt.Value <= agora && i.Ativo)
  .ToListAsync();
    }
}
