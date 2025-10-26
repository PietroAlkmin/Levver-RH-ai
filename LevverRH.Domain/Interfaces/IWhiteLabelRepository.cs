using LevverRH.Domain.Entities;
using LevverRH.Domain.Interfaces;

namespace LevverRH.Domain.Interfaces;

public interface IWhiteLabelRepository : IRepository<WhiteLabel>
{
    Task<WhiteLabel?> GetByTenantIdAsync(Guid tenantId);
    Task<WhiteLabel?> GetByDominioCustomizadoAsync(string dominio);
}