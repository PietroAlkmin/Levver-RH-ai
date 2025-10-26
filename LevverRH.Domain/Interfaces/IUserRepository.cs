using LevverRH.Domain.Entities;

namespace LevverRH.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByAzureAdIdAsync(string azureAdId);
    Task<IEnumerable<User>> GetByTenantIdAsync(Guid tenantId);
}