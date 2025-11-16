using LevverRH.Domain.Entities;
using LevverRH.Domain.Interfaces;
using LevverRH.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace LevverRH.Infra.Data.Repositories;

public class TenantRepository : Repository<Tenant>, ITenantRepository
{
    public TenantRepository(LevverDbContext context) : base(context)
    {
    }

    public async Task<Tenant?> GetByCnpjAsync(string cnpj)
    {
        return await _dbSet.FirstOrDefaultAsync(t => t.Cnpj == cnpj);
    }

    public async Task<Tenant?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(t => t.Email == email);
    }

    public async Task<Tenant?> GetByDominioAsync(string dominio)
    {
        return await _dbSet.FirstOrDefaultAsync(t => t.Dominio == dominio.ToLowerInvariant());
    }
}