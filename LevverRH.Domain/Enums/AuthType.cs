using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevverRH.Domain.Enums;

public enum AuthType
{
    Local = 1,      // Autenticação com email/senha
    AzureAd = 2     // Autenticação com Azure AD (SSO)
}
