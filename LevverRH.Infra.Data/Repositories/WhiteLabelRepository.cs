using LevverRH.Domain.Entities;
using LevverRH.Domain.Interfaces;
using LevverRH.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace LevverRH.Infra.Data.Repositories;

public class WhiteLabelRepository : Repository<WhiteLabel>, IWhiteLabelRepository
{
    public WhiteLabelRepository(LevverDbContext context) : base(context)
    {
    }

    public async Task<WhiteLabel?> GetByTenantIdAsync(Guid tenantId)
    {
        return await _dbSet
            .Include(w => w.Tenant)
            .FirstOrDefaultAsync(w => w.TenantId == tenantId);
    }

    public async Task<WhiteLabel?> GetByDominioCustomizadoAsync(string dominio)
    {
        return await _dbSet
            .FirstOrDefaultAsync(w => w.DominioCustomizado == dominio);
    }
}