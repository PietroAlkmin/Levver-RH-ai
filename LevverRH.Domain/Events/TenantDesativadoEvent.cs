using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevverRH.Domain.Events
{
    public class TenantDesativadoEvent
    {
        public Guid TenantId { get; }
        public DateTime DataDesativacao { get; }

        public TenantDesativadoEvent(Guid tenantId)
        {
            TenantId = tenantId;
            DataDesativacao = DateTime.UtcNow;
        }
    }
}
