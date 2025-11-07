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
        return await RegisterUserAsync(dto);
    }

    public async Task<ResultDTO<LoginResponseDTO>> RegisterTenantAsync(RegisterTenantRequestDTO dto)
    {
        try
        {
            // Verificar se email do admin já existe
            var existingAdmin = await _userRepository.GetByEmailAsync(dto.EmailAdmin);
            if (existingAdmin != null)
                return ResultDTO<LoginResponseDTO>.FailureResult("Email do administrador já cadastrado");

            // Verificar se CNPJ já existe
            var existingTenant = await _tenantRepository.GetByCnpjAsync(dto.Cnpj);
            if (existingTenant != null)
                return ResultDTO<LoginResponseDTO>.FailureResult("CNPJ já cadastrado");

            // Criar Tenant
            var tenant = new Tenant(
                nome: dto.NomeEmpresa,
                cnpj: dto.Cnpj,
                email: dto.EmailEmpresa,
                telefone: dto.TelefoneEmpresa,
                endereco: dto.EnderecoEmpresa
            );

            await _tenantRepository.AddAsync(tenant);

            // Hash da senha do admin
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Criar usuário Admin usando factory method
            var adminUser = User.CriarComSenha(
                tenantId: tenant.Id,
                email: dto.EmailAdmin,
                nome: dto.NomeAdmin,
                passwordHash: passwordHash,
                role: UserRole.Admin,
                tenant: tenant
            );

            await _userRepository.AddAsync(adminUser);

            // Buscar WhiteLabel (se existir)
            var whiteLabel = await _whiteLabelRepository.GetByTenantIdAsync(tenant.Id);

            // Gerar token JWT
            var token = GenerateJwtToken(adminUser);

            // Montar response
            var response = new LoginResponseDTO
            {
                Token = token,
                User = _mapper.Map<UserInfoDTO>(adminUser),
                Tenant = _mapper.Map<TenantInfoDTO>(tenant),
                WhiteLabel = whiteLabel != null ? _mapper.Map<WhiteLabelInfoDTO>(whiteLabel) : null
            };

            return ResultDTO<LoginResponseDTO>.SuccessResult(response, "Empresa e administrador criados com sucesso!");
        }
        catch (Exception ex)
        {
            return ResultDTO<LoginResponseDTO>.FailureResult($"Erro ao registrar empresa: {ex.Message}");
        }
    }

    public async Task<ResultDTO<LoginResponseDTO>> RegisterUserAsync(RegisterRequestDTO dto)
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
        try
        {
            // Validar token do Azure AD
            var azureClaims = await ValidateAzureAdTokenAsync(dto.AzureToken);
            
            if (azureClaims == null)
                return ResultDTO<LoginResponseDTO>.FailureResult("Token do Azure AD inválido");

            var email = azureClaims.FindFirst(ClaimTypes.Email)?.Value 
                ?? azureClaims.FindFirst("preferred_username")?.Value
                ?? azureClaims.FindFirst("upn")?.Value;

            if (string.IsNullOrEmpty(email))
                return ResultDTO<LoginResponseDTO>.FailureResult("Email não encontrado no token do Azure AD");

            var nome = azureClaims.FindFirst(ClaimTypes.Name)?.Value 
                ?? azureClaims.FindFirst("name")?.Value 
                ?? email.Split('@')[0];

            // Buscar usuário existente
            var user = await _userRepository.GetByEmailAsync(email);

            // Se usuário não existe, criar automaticamente
            if (user == null)
            {
                // Para simplificar, vamos exigir que o tenant já exista
                // O admin do tenant precisa convidar usuários Azure AD previamente
                return ResultDTO<LoginResponseDTO>.FailureResult(
                    "Usuário não cadastrado. Entre em contato com o administrador do sistema.");
            }
            else
            {
                // Verificar se usuário está configurado para Azure AD
                if (user.AuthType != AuthType.AzureAd)
                    return ResultDTO<LoginResponseDTO>.FailureResult("Este usuário não usa autenticação Azure AD");

                // Verificar se usuário está ativo
                if (!user.Ativo)
                    return ResultDTO<LoginResponseDTO>.FailureResult("Usuário inativo");

                // Verificar se tenant está ativo
                if (user.Tenant.Status != TenantStatus.Ativo)
                    return ResultDTO<LoginResponseDTO>.FailureResult("Tenant inativo");

                // Atualizar dados do usuário se necessário
                if (user.Nome != nome)
                {
                    user.AtualizarNome(nome);
                    await _userRepository.UpdateAsync(user);
                }
            }

            // Atualizar último login
            user.RegistrarLogin();
            await _userRepository.UpdateAsync(user);

            // Buscar WhiteLabel
            var whiteLabel = await _whiteLabelRepository.GetByTenantIdAsync(user.TenantId);

            // Gerar token JWT do sistema
            var token = GenerateJwtToken(user);

            // Montar response
            var response = new LoginResponseDTO
            {
                Token = token,
                User = _mapper.Map<UserInfoDTO>(user),
                Tenant = _mapper.Map<TenantInfoDTO>(user.Tenant),
                WhiteLabel = whiteLabel != null ? _mapper.Map<WhiteLabelInfoDTO>(whiteLabel) : null
            };

            return ResultDTO<LoginResponseDTO>.SuccessResult(response, "Login com Azure AD realizado com sucesso");
        }
        catch (Exception ex)
        {
            return ResultDTO<LoginResponseDTO>.FailureResult($"Erro ao realizar login com Azure AD: {ex.Message}");
        }
    }

    private async Task<ClaimsPrincipal?> ValidateAzureAdTokenAsync(string token)
    {
        try
        {
            var azureAdConfig = _configuration.GetSection("AzureAd");
            var tenantId = azureAdConfig["TenantId"];
            var clientId = azureAdConfig["ClientId"];

            // Configurar validação do token
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuers = new[]
                {
                    $"https://login.microsoftonline.com/{tenantId}/v2.0",
                    $"https://sts.windows.net/{tenantId}/",
                    "https://login.microsoftonline.com/common/v2.0" // Para multi-tenant
                },
                ValidateAudience = true,
                ValidAudiences = new[] { clientId, $"api://{clientId}" },
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = await GetAzureAdSigningKeysAsync()
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            return principal;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao validar token Azure AD: {ex.Message}");
            return null;
        }
    }

    private async Task<IEnumerable<SecurityKey>> GetAzureAdSigningKeysAsync()
    {
        var azureAdConfig = _configuration.GetSection("AzureAd");
        var tenantId = azureAdConfig["TenantId"] ?? "common";
        
        var stsDiscoveryEndpoint = $"https://login.microsoftonline.com/{tenantId}/v2.0/.well-known/openid-configuration";
        
        using var httpClient = new HttpClient();
        var response = await httpClient.GetStringAsync(stsDiscoveryEndpoint);
        var config = System.Text.Json.JsonDocument.Parse(response);
        var jwksUri = config.RootElement.GetProperty("jwks_uri").GetString();

        var jwksResponse = await httpClient.GetStringAsync(jwksUri!);
        var jwks = new Microsoft.IdentityModel.Tokens.JsonWebKeySet(jwksResponse);

        return jwks.GetSigningKeys();
    }

    public async Task<ResultDTO<string>> ResetPasswordAsync(string email, string newPassword)
    {
        try
        {
            // Buscar usuário por email
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
                return ResultDTO<string>.FailureResult("Usuário não encontrado");

            // Verificar se usa autenticação local
            if (user.AuthType != AuthType.Local)
                return ResultDTO<string>.FailureResult("Este usuário usa autenticação Azure AD. Não é possível redefinir senha.");

            // Hash da nova senha
            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            // Atualizar senha
            user.AtualizarSenha(newPasswordHash);
            await _userRepository.UpdateAsync(user);

            return ResultDTO<string>.SuccessResult("Senha redefinida com sucesso!");
        }
        catch (Exception ex)
        {
            return ResultDTO<string>.FailureResult($"Erro ao redefinir senha: {ex.Message}");
        }
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