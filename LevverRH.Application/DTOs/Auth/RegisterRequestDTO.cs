using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevverRH.Application.DTOs.Auth;

public class RegisterRequestDTO
{
    public Guid TenantId { get; set; }
    public string Email { get; set; } = null!;
    public string Nome { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
    public int Role { get; set; }  // 1=Admin, 2=Recruiter, 3=Viewer
}
