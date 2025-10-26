using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LevverRH.Domain.Entities;

namespace LevverRH.Domain.Interfaces
{
    public interface IAuditLogRepository : IRepository<AuditLog>
    {
        Task<IEnumerable<AuditLog>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<AuditLog>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<AuditLog>> GetByEntidadeAsync(string entidade, Guid entidadeId);
        Task<IEnumerable<AuditLog>> GetByPeriodoAsync(Guid tenantId, DateTime inicio, DateTime fim);
        Task<IEnumerable<AuditLog>> GetByAcaoAsync(Guid tenantId, string acao);
    }
}
