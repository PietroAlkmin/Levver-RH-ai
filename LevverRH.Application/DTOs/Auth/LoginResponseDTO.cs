using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevverRH.Application.DTOs.Auth;

public class LoginResponseDTO
{
    public string Token { get; set; } = null!;
    public UserInfoDTO User { get; set; } = null!;
    public TenantInfoDTO Tenant { get; set; } = null!;
    public WhiteLabelInfoDTO? WhiteLabel { get; set; }
}

public class UserInfoDTO
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string AuthType { get; set; } = null!;
  public string? FotoUrl { get; set; }
}

public class TenantInfoDTO
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Status { get; set; } = null!;
}

public class WhiteLabelInfoDTO
{
    public string? LogoUrl { get; set; }
    public string PrimaryColor { get; set; } = null!;
    public string SecondaryColor { get; set; } = null!;
    public string AccentColor { get; set; } = null!;
    public string BackgroundColor { get; set; } = null!;
    public string TextColor { get; set; } = null!;
    public string BorderColor { get; set; } = null!;
    public string SystemName { get; set; } = null!;
    public string? FaviconUrl { get; set; }
    public string? DominioCustomizado { get; set; }
}
