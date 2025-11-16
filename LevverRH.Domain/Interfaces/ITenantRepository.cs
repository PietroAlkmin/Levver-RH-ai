using LevverRH.Domain.Entities;
using LevverRH.Domain.Interfaces;

namespace LevverRH.Domain.Interfaces;

public interface ITenantRepository : IRepository<Tenant>
{
    Task<Tenant?> GetByCnpjAsync(string cnpj);
    Task<Tenant?> GetByEmailAsync(string email);
    Task<Tenant?> GetByDominioAsync(string dominio);
}