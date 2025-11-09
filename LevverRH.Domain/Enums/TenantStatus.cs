using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevverRH.Domain.Enums
{
    public enum TenantStatus
    {
        Ativo = 1,
        Inativo = 2,
        Suspenso = 3,
        PendenteSetup = 4  // Aguardando completar dados após primeiro login SSO
    }
}
