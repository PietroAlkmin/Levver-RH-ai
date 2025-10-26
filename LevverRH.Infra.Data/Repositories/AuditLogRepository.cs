using LevverRH.Domain.Entities;
using LevverRH.Domain.Interfaces;
using LevverRH.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace LevverRH.Infra.Data.Repositories;

public class AuditLogRepository : Repository<AuditLog>, IAuditLogRepository
{
    private const int DefaultLimit = 100;

    public AuditLogRepository(LevverDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<AuditLog>> GetByTenantIdAsync(Guid tenantId)
    {
        return await _dbSet
            .Where(a => a.TenantId == tenantId)
            .OrderByDescending(a => a.DataHora)
            .Take(DefaultLimit)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.DataHora)
            .Take(DefaultLimit)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetByEntidadeAsync(string entidade, Guid entidadeId)
    {
        return await _dbSet
            .Where(a => a.Entidade == entidade && a.EntidadeId == entidadeId)
            .OrderByDescending(a => a.DataHora)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetByPeriodoAsync(Guid tenantId, DateTime inicio, DateTime fim)
    {
        return await _dbSet
            .Where(a => a.TenantId == tenantId && a.DataHora >= inicio && a.DataHora <= fim)
            .OrderByDescending(a => a.DataHora)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetByAcaoAsync(Guid tenantId, string acao)
    {
        return await _dbSet
            .Where(a => a.TenantId == tenantId && a.Acao == acao)
            .OrderByDescending(a => a.DataHora)
            .Take(DefaultLimit)
            .ToListAsync();
    }
}