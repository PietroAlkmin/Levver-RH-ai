using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevverRH.Domain.Exceptions
{
    public class UnauthorizedException : DomainException
    {
        public UnauthorizedException(string message) : base(message)
        {
        }

        public UnauthorizedException()
            : base("Usuário não tem permissão para realizar esta operação.")
        {
        }
    }
}
