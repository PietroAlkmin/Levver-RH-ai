using LevverRH.Domain.Entities.Talents;
using LevverRH.Domain.Enums.Talents;

namespace LevverRH.Domain.Interfaces.Talents;

/// <summary>
/// Interface para repositório de Jobs (Vagas) do Talents
/// </summary>
public interface IJobRepository : IRepository<Job>
{
    /// <summary>
    /// Busca todas as vagas de um tenant
    /// </summary>
    Task<IEnumerable<Job>> GetByTenantIdAsync(Guid tenantId);

    /// <summary>
    /// Busca vagas por status
    /// </summary>
    Task<IEnumerable<Job>> GetByStatusAsync(Guid tenantId, JobStatus status);

    /// <summary>
    /// Busca vagas ativas (abertas) de um tenant
    /// </summary>
    Task<IEnumerable<Job>> GetActiveJobsAsync(Guid tenantId);

    /// <summary>
    /// Conta vagas ativas de um tenant
    /// </summary>
    Task<int> CountActiveJobsByTenantAsync(Guid tenantId);

    /// <summary>
    /// Busca vagas criadas por um usuário específico
    /// </summary>
    Task<IEnumerable<Job>> GetByCriadorAsync(Guid criadorId);

    /// <summary>
    /// Busca uma vaga específica de um tenant (validação de ownership)
    /// </summary>
    Task<Job?> GetByIdAndTenantAsync(Guid jobId, Guid tenantId);

    /// <summary>
    /// Busca vaga por ConversationId (para atualização progressiva com IA)
    /// </summary>
    Task<Job?> GetByConversationIdAsync(Guid conversationId);

    /// <summary>
    /// Busca vagas em rascunho de um usuário
    /// </summary>
    Task<IEnumerable<Job>> GetDraftsByUserAsync(Guid userId);
}
