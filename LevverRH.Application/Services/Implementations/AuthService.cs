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
            // 1️⃣ Validar token do Azure AD
            var azureClaims = await ValidateAzureAdTokenAsync(dto.AzureToken);
            
            if (azureClaims == null)
                return ResultDTO<LoginResponseDTO>.FailureResult("Token do Azure AD inválido");

            // 2️⃣ Extrair dados do token
            var email = azureClaims.FindFirst(ClaimTypes.Email)?.Value 
                ?? azureClaims.FindFirst("preferred_username")?.Value
                ?? azureClaims.FindFirst("upn")?.Value;

            if (string.IsNullOrEmpty(email))
                return ResultDTO<LoginResponseDTO>.FailureResult("Email não encontrado no token do Azure AD");

            var nome = azureClaims.FindFirst(ClaimTypes.Name)?.Value 
                ?? azureClaims.FindFirst("name")?.Value 
                ?? email.Split('@')[0];

            // Claims do Azure AD usam URIs completos, não nomes curtos
            var azureAdId = azureClaims.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value 
                ?? azureClaims.FindFirst("oid")?.Value 
                ?? azureClaims.FindFirst("sub")?.Value;

            var tenantIdMicrosoft = azureClaims.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid")?.Value
                ?? azureClaims.FindFirst("tid")?.Value;

            Console.WriteLine($"🔍 AzureAdId extraído do token: {azureAdId ?? "NULL"}");
            Console.WriteLine($"🔍 TenantIdMicrosoft extraído do token: {tenantIdMicrosoft ?? "NULL"}");

            // 3️⃣ Buscar usuário existente
            var user = await _userRepository.GetByEmailAsync(email);
            Console.WriteLine($"🔍 Buscou usuário por email {email}: {(user != null ? "ENCONTRADO" : "NÃO ENCONTRADO")}");

            if (user == null)
            {
                // 🆕 AUTO-PROVISIONING
                
                // Extrair domínio do email
                var emailParts = email.Split('@');
                if (emailParts.Length != 2)
                    return ResultDTO<LoginResponseDTO>.FailureResult("Email inválido");
                
                var dominio = emailParts[1].ToLowerInvariant();
                Console.WriteLine($"🔍 Domínio extraído: {dominio}");
                
                // Verificar domínios públicos (segurança)
                var dominiosPublicos = new[] { "gmail.com", "outlook.com", "hotmail.com", "yahoo.com", "live.com" };
                if (dominiosPublicos.Contains(dominio))
                {
                    return ResultDTO<LoginResponseDTO>.FailureResult(
                        "Não é possível criar conta com email pessoal. Use email corporativo.");
                }
                
                // Buscar tenant por domínio
                var tenant = await _tenantRepository.GetByDominioAsync(dominio);
                Console.WriteLine($"🔍 Buscou tenant por domínio {dominio}: {(tenant != null ? $"ENCONTRADO (Status={tenant.Status})" : "NÃO ENCONTRADO")}");
                
                if (tenant == null)
                {
                    // 🎯 PRIMEIRO LOGIN DO DOMÍNIO - Criar Tenant Pendente Setup
                    tenant = Tenant.CriarPendenteSetupViaSSO(dominio, email, tenantIdMicrosoft ?? string.Empty);
                    await _tenantRepository.AddAsync(tenant);
                    
                    // Criar usuário Admin (primeiro do tenant)
                    user = User.CriarComAzureAd(
                        tenantId: tenant.Id,
                        email: email,
                        nome: nome,
                        role: UserRole.Admin, // ⭐ Primeiro usuário = Admin
                        tenant: tenant,
                        azureAdId: azureAdId
                    );
                    
                    await _userRepository.AddAsync(user);
                    
                    Console.WriteLine($"✨ Tenant e Admin criados via SSO: {dominio} | {email}");
                }
                else
                {
                    // 🎯 TENANT JÁ EXISTE - Criar usuário comum
                    
                    // Verificar se tenant está ativo ou pendente setup
                    if (tenant.Status == TenantStatus.Inativo)
                        return ResultDTO<LoginResponseDTO>.FailureResult("Empresa inativa");
                    
                    if (tenant.Status == TenantStatus.Suspenso)
                        return ResultDTO<LoginResponseDTO>.FailureResult("Empresa suspensa");
                    
                    // Criar usuário comum (não admin)
                    user = User.CriarComAzureAd(
                        tenantId: tenant.Id,
                        email: email,
                        nome: nome,
                        role: UserRole.Viewer, // ⭐ Usuários subsequentes = Viewer
                        tenant: tenant,
                        azureAdId: azureAdId
                    );
                    
                    await _userRepository.AddAsync(user);
                    
                    Console.WriteLine($"✨ Usuário criado via SSO: {email}");
                }
            }
            else
            {
                // Usuário já existe - validar
                Console.WriteLine($"🔍 Usuário existente encontrado: {user.Email}, AuthType={user.AuthType}, Tenant.Status={user.Tenant.Status}");
                
                if (user.AuthType != AuthType.AzureAd)
                    return ResultDTO<LoginResponseDTO>.FailureResult(
                        "Este email já está cadastrado com autenticação por senha. Use login convencional.");

                if (!user.Ativo)
                    return ResultDTO<LoginResponseDTO>.FailureResult("Usuário inativo");

                // ✅ Permitir login se Ativo OU PendenteSetup (admin precisa completar setup)
                if (user.Tenant.Status == TenantStatus.Inativo)
                {
                    Console.WriteLine($"❌ BLOQUEADO: Tenant Inativo");
                    return ResultDTO<LoginResponseDTO>.FailureResult("Empresa inativa");
                }

                if (user.Tenant.Status == TenantStatus.Suspenso)
                {
                    Console.WriteLine($"❌ BLOQUEADO: Tenant Suspenso");
                    return ResultDTO<LoginResponseDTO>.FailureResult("Empresa suspensa");
                }
                
                Console.WriteLine($"✅ Validações OK - Tenant.Status={user.Tenant.Status} é válido");

                // Atualizar nome se mudou no Azure AD
                if (user.Nome != nome)
                {
                    user.AtualizarNome(nome);
                    await _userRepository.UpdateAsync(user);
                }
            }

            // 4️⃣ Registrar login
            user.RegistrarLogin();
            await _userRepository.UpdateAsync(user);

            // 5️⃣ Buscar WhiteLabel
            var whiteLabel = await _whiteLabelRepository.GetByTenantIdAsync(user.TenantId);

            // 6️⃣ Gerar token JWT
            var token = GenerateJwtToken(user);

            // 7️⃣ Montar response
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
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // ✅ Adicionar para compatibilidade
            new Claim(JwtClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Email, user.Email), // ✅ Adicionar para compatibilidade
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

    public async Task<ResultDTO<string>> CompleteTenantSetupAsync(Guid userId, CompleteTenantSetupDTO dto)
    {
        try
        {
            Console.WriteLine($"🔹 CompleteTenantSetupAsync - userId: {userId}");
            
            // Buscar usuário
            var user = await _userRepository.GetByIdAsync(userId);
            
            if (user == null)
            {
                Console.WriteLine("❌ Usuário não encontrado");
                return ResultDTO<string>.FailureResult("Usuário não encontrado");
            }

            Console.WriteLine($"🔹 Usuário encontrado: {user.Email}, TenantId: {user.TenantId}");

            // Verificar se é admin
            if (user.Role != UserRole.Admin)
            {
                Console.WriteLine($"❌ Usuário não é admin: Role={user.Role}");
                return ResultDTO<string>.FailureResult("Apenas o administrador pode completar o setup");
            }

            // Buscar tenant separadamente (GetByIdAsync não faz Include do Tenant)
            var tenant = await _tenantRepository.GetByIdAsync(user.TenantId);
            
            if (tenant == null)
            {
                Console.WriteLine("❌ Tenant não encontrado");
                return ResultDTO<string>.FailureResult("Empresa não encontrada");
            }

            Console.WriteLine($"🔹 Tenant encontrado: {tenant.Nome}, Status: {tenant.Status}");

            // Verificar se tenant está pendente setup
            if (tenant.Status != TenantStatus.PendenteSetup)
            {
                Console.WriteLine($"❌ Tenant não está em PendenteSetup: Status={tenant.Status}");
                return ResultDTO<string>.FailureResult("Tenant já foi configurado");
            }

            // Verificar se CNPJ já existe
            var existingTenant = await _tenantRepository.GetByCnpjAsync(dto.Cnpj);
            if (existingTenant != null && existingTenant.Id != tenant.Id)
            {
                Console.WriteLine($"❌ CNPJ já existe: {dto.Cnpj}");
                return ResultDTO<string>.FailureResult("CNPJ já cadastrado para outra empresa");
            }

            Console.WriteLine($"🔹 Atualizando tenant...");

            // Atualizar dados do tenant
            tenant.AtualizarNome(dto.NomeEmpresa);
            tenant.AtualizarCnpj(dto.Cnpj);
            tenant.AtualizarEmail(dto.EmailEmpresa);
            tenant.AtualizarTelefone(dto.TelefoneEmpresa);
            tenant.AtualizarEndereco(dto.EnderecoEmpresa);
            tenant.Ativar(); // Status = Ativo

            await _tenantRepository.UpdateAsync(tenant);
            
            Console.WriteLine($"✅ Tenant setup concluído: {tenant.Nome} | Status={tenant.Status}");

            return ResultDTO<string>.SuccessResult("Setup concluído com sucesso! Sua empresa está ativa.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao completar setup: {ex.Message}");
            Console.WriteLine($"❌ StackTrace: {ex.StackTrace}");
            return ResultDTO<string>.FailureResult($"Erro ao completar setup: {ex.Message}");
        }
    }
}