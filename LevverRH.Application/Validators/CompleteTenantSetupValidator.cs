using FluentValidation;
using LevverRH.Application.DTOs.Auth;

namespace LevverRH.Application.Validators;

public class CompleteTenantSetupValidator : AbstractValidator<CompleteTenantSetupDTO>
{
    public CompleteTenantSetupValidator()
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
