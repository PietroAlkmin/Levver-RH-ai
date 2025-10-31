using FluentValidation;
using LevverRH.Application.DTOs.Auth;

namespace LevverRH.Application.Validators;

public class RegisterTenantRequestValidator : AbstractValidator<RegisterTenantRequestDTO>
{
    public RegisterTenantRequestValidator()
    {
        // Validações da Empresa
        RuleFor(x => x.NomeEmpresa)
            .NotEmpty().WithMessage("Nome da empresa é obrigatório")
            .MaximumLength(255).WithMessage("Nome da empresa deve ter no máximo 255 caracteres");

        RuleFor(x => x.Cnpj)
            .NotEmpty().WithMessage("CNPJ é obrigatório")
            .Must(BeValidCnpj).WithMessage("CNPJ deve ter 14 dígitos");

        RuleFor(x => x.EmailEmpresa)
            .NotEmpty().WithMessage("Email da empresa é obrigatório")
            .EmailAddress().WithMessage("Email da empresa inválido")
            .MaximumLength(255).WithMessage("Email deve ter no máximo 255 caracteres");

        RuleFor(x => x.TelefoneEmpresa)
            .MaximumLength(20).WithMessage("Telefone deve ter no máximo 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.TelefoneEmpresa));

        // Validações do Admin
        RuleFor(x => x.NomeAdmin)
            .NotEmpty().WithMessage("Nome do administrador é obrigatório")
            .MaximumLength(255).WithMessage("Nome deve ter no máximo 255 caracteres");

        RuleFor(x => x.EmailAdmin)
            .NotEmpty().WithMessage("Email do administrador é obrigatório")
            .EmailAddress().WithMessage("Email do administrador inválido")
            .MaximumLength(255).WithMessage("Email deve ter no máximo 255 caracteres");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Senha é obrigatória")
            .MinimumLength(6).WithMessage("Senha deve ter no mínimo 6 caracteres")
            .MaximumLength(100).WithMessage("Senha deve ter no máximo 100 caracteres")
            .Matches(@"[A-Z]").WithMessage("Senha deve conter pelo menos uma letra maiúscula")
            .Matches(@"[a-z]").WithMessage("Senha deve conter pelo menos uma letra minúscula")
            .Matches(@"[0-9]").WithMessage("Senha deve conter pelo menos um número");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirmação de senha é obrigatória")
            .Equal(x => x.Password).WithMessage("Senhas não conferem");
    }

    private bool BeValidCnpj(string? cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            return false;

        // Remove tudo que não é dígito
        var apenasNumeros = new string(cnpj.Where(char.IsDigit).ToArray());
        
        // Valida se tem exatamente 14 dígitos
        return apenasNumeros.Length == 14;
    }
}
