namespace LevverRH.Application.DTOs.Auth;

public class ResetPasswordRequestDTO
{
    public string Email { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}
