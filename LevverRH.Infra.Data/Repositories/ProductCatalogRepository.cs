using LevverRH.Domain.Entities;
using LevverRH.Domain.Enums;
using LevverRH.Domain.Interfaces;
using LevverRH.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace LevverRH.Infra.Data.Repositories;

public class ProductCatalogRepository : Repository<ProductCatalog>, IProductCatalogRepository
{
    public ProductCatalogRepository(LevverDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ProductCatalog>> GetAtivosPorCategoriaAsync(string categoria)
    {
        return await _dbSet
            .Where(p => p.Ativo && p.Categoria == categoria)
            .ToListAsync();
    }

    public async Task<ProductCatalog?> GetByNomeAsync(string nome)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.Nome == nome);
    }

    public async Task<IEnumerable<ProductCatalog>> GetByModeloCobrancaAsync(ModeloCobranca modelo)
    {
        return await _dbSet
            .Where(p => p.ModeloCobranca == modelo)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductCatalog>> GetAtivosAsync()
    {
        return await _dbSet
            .Where(p => p.Ativo)
            .ToListAsync();
    }
}