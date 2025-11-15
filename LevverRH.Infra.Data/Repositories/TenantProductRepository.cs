using LevverRH.Domain.Entities;
using LevverRH.Domain.Interfaces;
using LevverRH.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace LevverRH.Infra.Data.Repositories;

public class TenantProductRepository : Repository<TenantProduct>, ITenantProductRepository
{
    public TenantProductRepository(LevverDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TenantProduct>> GetByTenantIdAsync(Guid tenantId)
    {
        return await _dbSet
            .Include(tp => tp.Product)
            .Where(tp => tp.TenantId == tenantId)
            .ToListAsync();
    }

    public async Task<IEnumerable<TenantProduct>> GetActiveTenantProductsAsync(Guid tenantId)
    {
        return await _dbSet
            .Include(tp => tp.Product)
            .Where(tp => tp.TenantId == tenantId && tp.Ativo)
            .ToListAsync();
    }

    public async Task<TenantProduct?> GetByTenantAndProductAsync(Guid tenantId, Guid productId)
    {
        return await _dbSet
            .Include(tp => tp.Product)
            .FirstOrDefaultAsync(tp => tp.TenantId == tenantId && tp.ProductId == productId);
    }

    public async Task<bool> HasAccessToProductAsync(Guid tenantId, Guid productId)
    {
        return await _dbSet
            .AnyAsync(tp => tp.TenantId == tenantId && tp.ProductId == productId && tp.Ativo);
    }
}
