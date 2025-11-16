using LevverRH.Domain.Entities;

namespace LevverRH.Domain.Interfaces;

public interface ITenantProductRepository : IRepository<TenantProduct>
{
    Task<IEnumerable<TenantProduct>> GetByTenantIdAsync(Guid tenantId);
    Task<IEnumerable<TenantProduct>> GetActiveTenantProductsAsync(Guid tenantId);
    Task<TenantProduct?> GetByTenantAndProductAsync(Guid tenantId, Guid productId);
    Task<bool> HasAccessToProductAsync(Guid tenantId, Guid productId);
}
