using LevverRH.Domain.Entities;
using LevverRH.Domain.Interfaces;

namespace LevverRH.Domain.Interfaces;

public interface ITenantSubscriptionRepository : IRepository<TenantSubscription>
{
    Task<IEnumerable<TenantSubscription>> GetByTenantIdAsync(Guid tenantId);
    Task<TenantSubscription?> GetSubscriptionAtivaAsync(Guid tenantId, Guid productId);
    Task<IEnumerable<TenantSubscription>> GetSubscricoesVigentesAsync(Guid tenantId);
}