using AutoMapper;
using BCrypt.Net;
using LevverRH.Application.DTOs.Auth;
using LevverRH.Application.DTOs.Common;
using LevverRH.Application.Services.Interfaces;
using LevverRH.Domain.Entities;
using LevverRH.Domain.Enums;
using LevverRH.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace LevverRH.Application.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IWhiteLabelRepository _whiteLabelRepository;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUserRepository userRepository,
        ITenantRepository tenantRepository,
        IWhiteLabelRepository whiteLabelRepository,
        IMapper mapper,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _tenantRepository = tenantRepository;
        _whiteLabelRepository = whiteLabelRepository;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<ResultDTO<LoginResponseDTO>> LoginAsync(LoginRequestDTO dto)
    {
        try
        {
            // Buscar usuário por email
            var user = await _userRepository.GetByEmailAsync(dto.Email);

            if (user == null)
                return ResultDTO<LoginResponseDTO>.FailureResult("Email ou senha inválidos");

            // Verificar se é usuário local
            if (user.AuthType != AuthType.Local)
                return ResultDTO<LoginResponseDTO>.FailureResult("Este usuário usa autenticação externa (Azure AD)");

            // Validar senha
            if (!user.ValidatePassword(dto.Password))
                return ResultDTO<LoginResponseDTO>.FailureResult("Email ou senha inválidos");

            // Verificar se usuário está ativo
            if (!user.Ativo)
                return ResultDTO<LoginResponseDTO>.FailureResult("Usuário inativo");

            // Verificar se tenant está ativo
            if (user.Tenant.Status != TenantStatus.Ativo)
                return ResultDTO<LoginResponseDTO>.FailureResult("Tenant inativo");

            // Atualizar último login
            user.RegistrarLogin();
            await _userRepository.UpdateAsync(user);

            // Buscar WhiteLabel
            var whiteLabel = await _whiteLabelRepository.GetByTenantIdAsync(user.TenantId);

            // Gerar token JWT
            var token = GenerateJwtToken(user);

            // Montar response
            var response = new LoginResponseDTO
            {
                Token = token,
                User = _mapper.Map<UserInfoDTO>(user),
                Tenant = _mapper.Map<TenantInfoDTO>(user.Tenant),
                WhiteLabel = whiteLabel != null ? _mapper.Map<WhiteLabelInfoDTO>(whiteLabel) : null
            };

            return ResultDTO<LoginResponseDTO>.SuccessResult(response, "Login realizado com sucesso");
        }
        catch (Exception ex)
        {
            return ResultDTO<LoginResponseDTO>.FailureResult($"Erro ao realizar login: {ex.Message}");
        }
    }

    public async Task<ResultDTO<LoginResponseDTO>> RegisterAsync(RegisterRequestDTO dto)
    {
        try
        {
            // Verificar se email já existe
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
                return ResultDTO<LoginResponseDTO>.FailureResult("Email já cadastrado");

            // Buscar tenant
            var tenant = await _tenantRepository.GetByIdAsync(dto.TenantId);
            if (tenant == null)
                return ResultDTO<LoginResponseDTO>.FailureResult("Tenant não encontrado");

            if (tenant.Status != TenantStatus.Ativo)
                return ResultDTO<LoginResponseDTO>.FailureResult("Tenant inativo");

            // Hash da senha
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Criar usuário usando factory method
            var user = User.CriarComSenha(
                tenantId: dto.TenantId,
                email: dto.Email,
                nome: dto.Nome,
                passwordHash: passwordHash,
                role: (UserRole)dto.Role,
                tenant: tenant
            );

            await _userRepository.AddAsync(user);

            // Buscar WhiteLabel
            var whiteLabel = await _whiteLabelRepository.GetByTenantIdAsync(user.TenantId);

            // Gerar token JWT
            var token = GenerateJwtToken(user);

            // Montar response
            var response = new LoginResponseDTO
            {
                Token = token,
                User = _mapper.Map<UserInfoDTO>(user),
                Tenant = _mapper.Map<TenantInfoDTO>(user.Tenant),
                WhiteLabel = whiteLabel != null ? _mapper.Map<WhiteLabelInfoDTO>(whiteLabel) : null
            };

            return ResultDTO<LoginResponseDTO>.SuccessResult(response, "Usuário criado com sucesso");
        }
        catch (Exception ex)
        {
            return ResultDTO<LoginResponseDTO>.FailureResult($"Erro ao registrar usuário: {ex.Message}");
        }
    }

    public async Task<ResultDTO<LoginResponseDTO>> LoginWithAzureAdAsync(AzureAdLoginRequestDTO dto)
    {
        // TODO: Implementar login com Azure AD
        await Task.CompletedTask;
        return ResultDTO<LoginResponseDTO>.FailureResult("Login com Azure AD ainda não implementado");
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "60");

        var claims = new[]
        {
            new Claim(JwtClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Nome),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("TenantId", user.TenantId.ToString()),
            new Claim("AuthType", user.AuthType.ToString()),
            new Claim(JwtClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}