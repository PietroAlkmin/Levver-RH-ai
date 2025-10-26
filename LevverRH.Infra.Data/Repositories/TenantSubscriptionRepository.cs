using LevverRH.Domain.Entities;
using LevverRH.Domain.Enums;
using LevverRH.Domain.Interfaces;
using LevverRH.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace LevverRH.Infra.Data.Repositories;

public class TenantSubscriptionRepository : Repository<TenantSubscription>, ITenantSubscriptionRepository
{
    public TenantSubscriptionRepository(LevverDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TenantSubscription>> GetByTenantIdAsync(Guid tenantId)
    {
        return await _dbSet
            .Include(ts => ts.Tenant)
 .Include(ts => ts.ProductCatalog)
  .Where(ts => ts.TenantId == tenantId)
     .ToListAsync();
    }

    public async Task<TenantSubscription?> GetSubscriptionAtivaAsync(Guid tenantId, Guid productId)
    {
     var hoje = DateTime.UtcNow.Date;
    
        return await _dbSet
            .Include(ts => ts.Tenant)
          .Include(ts => ts.ProductCatalog)
     .FirstOrDefaultAsync(ts =>
          ts.TenantId == tenantId &&
    ts.ProductCatalogId == productId &&
         ts.Status == SubscriptionStatus.Ativo &&
     ts.DataInicio.Date <= hoje &&
           (ts.DataFim == null || ts.DataFim.Value.Date >= hoje));
    }

    public async Task<IEnumerable<TenantSubscription>> GetSubscricoesVigentesAsync(Guid tenantId)
    {
        var hoje = DateTime.UtcNow.Date;
        
        return await _dbSet
            .Include(ts => ts.Tenant)
 .Include(ts => ts.ProductCatalog)
     .Where(ts => ts.TenantId == tenantId 
   && ts.Status == SubscriptionStatus.Ativo
            && ts.DataInicio.Date <= hoje
      && (ts.DataFim == null || ts.DataFim.Value.Date >= hoje))
 .ToListAsync();
    }
}