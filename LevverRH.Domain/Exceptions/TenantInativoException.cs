using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevverRH.Domain.Exceptions
{
    public class TenantInativoException : DomainException
    {
        public TenantInativoException()
            : base("Tenant está inativo e não pode realizar operações.")
        {
        }

        public TenantInativoException(Guid tenantId)
            : base($"Tenant {tenantId} está inativo e não pode realizar operações.")
        {
        }
    }
}
