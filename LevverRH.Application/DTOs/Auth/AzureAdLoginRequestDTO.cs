using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevverRH.Application.DTOs.Auth;

public class AzureAdLoginRequestDTO
{
    public string AzureToken { get; set; } = null!;
}
