using LevverRH.Domain.Entities;

namespace LevverRH.Domain.Interfaces;

public interface IIntegrationCredentialsRepository : IRepository<IntegrationCredentials>
{
    Task<IEnumerable<IntegrationCredentials>> GetByTenantIdAsync(Guid tenantId);
    Task<IntegrationCredentials?> GetByTenantAndPlataformaAsync(Guid tenantId, string plataforma);
    Task<IEnumerable<IntegrationCredentials>> GetTokensExpiradosAsync();
}
