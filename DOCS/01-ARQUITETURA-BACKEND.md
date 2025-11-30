# 🏗️ Arquitetura Backend - Levver.ai RH

## 📐 Clean Architecture Pattern

O backend segue os princípios da **Clean Architecture** (Robert C. Martin), garantindo:
- **Independência de Frameworks**
- **Testabilidade**
- **Independência de UI**
- **Independência de Banco de Dados**
- **Independência de Agentes Externos**

## 🔷 Camadas da Arquitetura

### **1️⃣ LevverRH.Domain (Camada de Domínio)**

**Responsabilidade**: Contém as regras de negócio e entidades do domínio.

#### **Estrutura**

```
LevverRH.Domain/
├── Entities/                    # Entidades do domínio
│   ├── User.cs                 # Usuário do sistema
│   ├── Tenant.cs               # Empresa (multi-tenant)
│   ├── ProductCatalog.cs       # Produto disponível no catálogo
│   ├── TenantProduct.cs        # Produto contratado por tenant
│   ├── TenantSubscription.cs   # Assinatura ativa
│   ├── WhiteLabel.cs           # Personalização visual
│   ├── IntegrationCredentials.cs  # Credenciais de APIs
│   ├── AuditLog.cs             # Log de auditoria
│   │
│   └── Talents/                # 🎯 Levver Talents (Recrutamento)
│       ├── Vaga.cs             # Vagas de emprego
│       ├── Candidatura.cs      # Candidaturas
│       ├── Entrevista.cs       # Entrevistas agendadas
│       ├── Avaliacao.cs        # Avaliações de candidatos
│       ├── Etapa.cs            # Pipeline de recrutamento
│       └── Habilidade.cs       # Skills e competências
│
├── Enums/                      # Enumerações
│   ├── AuthType.cs             # EmailSenha, AzureAd
│   ├── UserRole.cs             # Admin, Manager, User
│   ├── TenantStatus.cs         # Ativo, Inativo, Suspenso
│   ├── SubscriptionStatus.cs  # Ativa, Cancelada, Suspensa
│   ├── ModeloCobranca.cs       # Mensal, Anual, Unico, Uso
│   │
│   └── Talents/                # Enums do Levver Talents
│       ├── StatusVaga.cs       # Aberta, Fechada, Suspensa, Cancelada
│       ├── StatusCandidatura.cs  # Nova, EmAnalise, Entrevista, Aprovada, etc
│       ├── TipoEntrevista.cs   # Presencial, Online, Telefone
│       └── StatusEntrevista.cs # Agendada, Realizada, Cancelada
│
├── Interfaces/                 # Contratos de repositórios
│   ├── IRepository.cs          # Repository genérico
│   ├── IUserRepository.cs
│   ├── ITenantRepository.cs
│   ├── IProductCatalogRepository.cs
│   ├── ITenantProductRepository.cs
│   ├── ICandidateAnalyzer.cs   # 🤖 Análise de currículos com IA
│   ├── IPdfExtractor.cs        # 📄 Extração de texto de PDF
│   └── ... (outros repositories)
│
├── Events/                     # Domain Events
│   ├── SubscriptionCanceledEvent.cs
│   ├── TenantDesativadoEvent.cs
│   └── UserRoleChangedEvent.cs
│
└── Exceptions/                 # Exceções de domínio
    ├── DomainException.cs
    ├── TenantInativoException.cs
    └── UnauthorizedException.cs
```

#### **Entidades Principais**

##### **User.cs**
```csharp
public class User
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string Nome { get; private set; }
    public string Email { get; private set; }
    public string? PasswordHash { get; private set; }
    public UserRole Role { get; private set; }
    public AuthType AuthType { get; private set; }
    public string? AzureAdObjectId { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime DataCriacao { get; private set; }
    
    // Navigation Properties
    public virtual Tenant Tenant { get; set; }
    
    // Domain Methods
    public void Ativar() { ... }
    public void Desativar() { ... }
    public void AlterarRole(UserRole novaRole) { ... }
    public void AtualizarSenha(string novaSenha) { ... }
}
```

##### **ProductCatalog.cs**
```csharp
public class ProductCatalog
{
    public Guid Id { get; private set; }
    public string ProdutoNome { get; private set; }
    public string Descricao { get; private set; }
    public string Categoria { get; private set; }
    public string? Icone { get; private set; }
    public string? CorPrimaria { get; private set; }
    public string? RotaBase { get; private set; }
    public int OrdemExibicao { get; private set; }
    public bool Lancado { get; private set; }
    public ModeloCobranca ModeloCobranca { get; private set; }
    public decimal ValorBasePadrao { get; private set; }
    public bool Ativo { get; private set; }
    
    // Navigation Properties
    public virtual ICollection<TenantProduct> TenantProducts { get; set; }
    
    // Domain Methods
    public void MarcarComoLancado() { ... }
    public void MarcarComoEmBreve() { ... }
    public void AtualizarVisualizacao(string icone, string cor, int ordem) { ... }
}
```

##### **TenantProduct.cs** (Tabela Associativa com Propriedades)
```csharp
public class TenantProduct
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid ProductId { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime? DataAtivacao { get; private set; }
    public DateTime? DataDesativacao { get; private set; }
    public string? ConfiguracaoJson { get; private set; }
    public DateTime DataCriacao { get; private set; }
    
    // Navigation Properties
    public virtual Tenant Tenant { get; set; }
    public virtual ProductCatalog Product { get; set; }
    
    // Domain Methods
    public void Ativar() { ... }
    public void Desativar() { ... }
    public void AtualizarConfiguracao(string json) { ... }
}
```

---

### **2️⃣ LevverRH.Application (Camada de Aplicação)**

**Responsabilidade**: Orquestra os casos de uso da aplicação.

#### **Estrutura**

```
LevverRH.Application/
├── Services/
│   ├── Interfaces/
│   │   ├── IAuthService.cs
│   │   ├── IProductService.cs
│   │   ├── IJobAIService.cs        # 🤖 Criação de vagas com IA
│   │   ├── ICandidateAnalyzer.cs   # 🤖 Análise de currículos
│   │   ├── IPdfExtractor.cs        # 📄 Extração de PDF
│   │   └── ...
│   └── Implementations/
│       ├── AuthService.cs       # Login, Register, SSO
│       ├── ProductService.cs    # Gestão de produtos
│       ├── JobAIService.cs      # 🤖 IA para criação de vagas
│       ├── CandidateAnalyzer.cs # 🤖 IA para análise de currículos
│       ├── PdfExtractor.cs      # 📄 Extração de texto PDF
│       └── ...
│
├── DTOs/                        # Data Transfer Objects
│   ├── Auth/
│   │   ├── LoginRequestDTO.cs
│   │   ├── LoginResponseDTO.cs
│   │   ├── RegisterRequestDTO.cs
│   │   └── AzureAdLoginRequestDTO.cs
│   ├── Product/
│   │   ├── ProductDTO.cs
│   │   └── TenantProductDTO.cs
│   ├── Talents/                 # 🎯 DTOs do Levver Talents
│   │   ├── JobDTO.cs
│   │   ├── ApplicationDTO.cs
│   │   ├── AnalyzeCandidateResponseDTO.cs  # 🤖 Resultado análise IA
│   │   └── ...
│   └── Common/
│       └── ResultDTO.cs
│
├── Mappings/                    # AutoMapper Profiles
│   ├── AuthMappingProfile.cs
│   └── ProductMappingProfile.cs
│
└── Validators/                  # FluentValidation
    ├── LoginRequestValidator.cs
    └── RegisterRequestValidator.cs
```

#### **Services**

##### **AuthService.cs**
```csharp
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    private readonly ITenantRepository _tenantRepo;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtGenerator;
    
    // Login com Email/Senha
    public async Task<ResultDTO<LoginResponseDTO>> LoginAsync(LoginRequestDTO request)
    {
        // 1. Buscar usuário por email
        // 2. Validar senha
        // 3. Verificar tenant ativo
        // 4. Gerar JWT token
        // 5. Retornar dados de autenticação
    }
    
    // Login com Azure AD
    public async Task<ResultDTO<LoginResponseDTO>> LoginWithAzureAdAsync(AzureAdLoginRequestDTO request)
    {
        // 1. Validar token do Azure
        // 2. Buscar usuário por AzureAdObjectId
        // 3. Se não existe:
        //    a. Criar tenant (se novo)
        //    b. Criar usuário admin
        //    c. Retornar token parcial para completar setup
        // 4. Se existe:
        //    a. Verificar tenant ativo
        //    b. Gerar JWT token completo
        //    c. Retornar dados de autenticação
    }
    
    // Completar Setup de Tenant (SSO)
    public async Task<ResultDTO<LoginResponseDTO>> CompleteTenantSetupAsync(CompleteTenantSetupDTO request)
    {
        // 1. Buscar tenant do usuário logado
        // 2. Atualizar dados da empresa
        // 3. Ativar tenant
        // 4. Gerar novo JWT token (completo)
        // 5. Retornar dados atualizados
    }
}
```

##### **ProductService.cs**
```csharp
public class ProductService : IProductService
{
    private readonly IProductCatalogRepository _productRepo;
    private readonly ITenantProductRepository _tenantProductRepo;
    
    // Listar todos os produtos do catálogo
    public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
    {
        var products = await _productRepo.GetAllActiveAsync();
        return _mapper.Map<IEnumerable<ProductDTO>>(products);
    }
    
    // Listar produtos contratados pelo tenant
    public async Task<IEnumerable<TenantProductDTO>> GetMyProductsAsync(Guid tenantId)
    {
        var tenantProducts = await _tenantProductRepo.GetByTenantIdAsync(tenantId);
        return _mapper.Map<IEnumerable<TenantProductDTO>>(tenantProducts);
    }
    
    // Verificar se tenant tem acesso a um produto
    public async Task<bool> HasAccessToProductAsync(Guid tenantId, Guid productId)
    {
        var tenantProduct = await _tenantProductRepo.GetByTenantAndProductAsync(tenantId, productId);
        return tenantProduct != null && tenantProduct.Ativo;
    }
}
```

---

### **3️⃣ LevverRH.Infra.Data (Camada de Infraestrutura)**

**Responsabilidade**: Implementação de persistência de dados.

#### **Estrutura**

```
LevverRH.Infra.Data/
├── Context/
│   └── ApplicationDbContext.cs  # DbContext do EF Core
│
├── Repositories/                # Implementação de IRepository
│   ├── Repository.cs            # Repository genérico
│   ├── UserRepository.cs
│   ├── TenantRepository.cs
│   ├── ProductCatalogRepository.cs
│   ├── TenantProductRepository.cs
│   └── ...
│
├── EntitiesConfiguration/       # Fluent API do EF Core
│   ├── UserConfiguration.cs
│   ├── TenantConfiguration.cs
│   ├── ProductCatalogConfiguration.cs
│   ├── TenantProductConfiguration.cs
│   └── ...
│
├── Migrations/                  # EF Core Migrations
│   ├── 20251114000000_InitialCreate.cs
│   └── 20251114021237_AddTenantProductsTable.cs
│
└── Seed/                        # Dados iniciais
    └── SeedData.cs
```

#### **ApplicationDbContext.cs**

```csharp
public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<ProductCatalog> ProductsCatalog { get; set; }
    public DbSet<TenantProduct> TenantProducts { get; set; }
    public DbSet<TenantSubscription> TenantSubscriptions { get; set; }
    public DbSet<WhiteLabel> WhiteLabels { get; set; }
    public DbSet<IntegrationCredentials> IntegrationCredentials { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Aplicar todas as configurações de entidades
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        
        // Definir schema padrão como "shared"
        modelBuilder.HasDefaultSchema("shared");
    }
}
```

#### **Configurações de Entidades (Fluent API)**

##### **ProductCatalogConfiguration.cs**
```csharp
public class ProductCatalogConfiguration : IEntityTypeConfiguration<ProductCatalog>
{
    public void Configure(EntityTypeBuilder<ProductCatalog> builder)
    {
        builder.ToTable("products_catalog", "shared");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.ProdutoNome)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("produto_nome");
            
        builder.Property(p => p.Descricao)
            .HasMaxLength(500)
            .HasColumnName("descricao");
            
        builder.Property(p => p.Icone)
            .HasMaxLength(50)
            .HasColumnName("icone");
            
        builder.Property(p => p.CorPrimaria)
            .HasMaxLength(7)
            .HasColumnName("cor_primaria");
            
        builder.Property(p => p.RotaBase)
            .HasMaxLength(100)
            .HasColumnName("rota_base");
            
        builder.Property(p => p.OrdemExibicao)
            .HasColumnName("ordem_exibicao");
            
        builder.Property(p => p.Lancado)
            .HasColumnName("lancado");
            
        builder.Property(p => p.ValorBasePadrao)
            .HasColumnType("decimal(10,2)")
            .HasColumnName("ValorBasePadrao");
            
        // Relacionamento com TenantProducts
        builder.HasMany(p => p.TenantProducts)
            .WithOne(tp => tp.Product)
            .HasForeignKey(tp => tp.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

##### **TenantProductConfiguration.cs**
```csharp
public class TenantProductConfiguration : IEntityTypeConfiguration<TenantProduct>
{
    public void Configure(EntityTypeBuilder<TenantProduct> builder)
    {
        builder.ToTable("tenant_products", "shared");
        
        builder.HasKey(tp => tp.Id);
        
        // Índice único composto (tenant_id + product_id)
        builder.HasIndex(tp => new { tp.TenantId, tp.ProductId })
            .IsUnique()
            .HasDatabaseName("IX_tenant_products_tenant_product");
            
        // Foreign Keys
        builder.HasOne(tp => tp.Tenant)
            .WithMany(t => t.TenantProducts)
            .HasForeignKey(tp => tp.TenantId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(tp => tp.Product)
            .WithMany(p => p.TenantProducts)
            .HasForeignKey(tp => tp.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.Property(tp => tp.ConfiguracaoJson)
            .HasColumnType("nvarchar(max)")
            .HasColumnName("configuracao_json");
    }
}
```

---

### **4️⃣ LevverRH.Infra.IoC (Dependency Injection)**

**Responsabilidade**: Configuração de injeção de dependências.

```csharp
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        
        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IProductCatalogRepository, ProductCatalogRepository>();
        services.AddScoped<ITenantProductRepository, TenantProductRepository>();
        
        // Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProductService, ProductService>();
        
        // AutoMapper
        services.AddAutoMapper(typeof(AuthMappingProfile).Assembly);
        
        // FluentValidation
        services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
        
        return services;
    }
}
```

---

### **5️⃣ LevverRH.WebApp (Camada de Apresentação)**

**Responsabilidade**: Controllers e configuração da API.

#### **Estrutura**

```
LevverRH.WebApp/
├── Controllers/
│   ├── AuthController.cs        # /api/auth/*
│   ├── ProductsController.cs    # /api/products/*
│   └── ...
│
├── Program.cs                   # Configuração da aplicação
├── appsettings.json             # Configurações
└── appsettings.Development.json # Configurações de dev
```

#### **AuthController.cs**

```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    
    // POST /api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
    {
        var result = await _authService.LoginAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }
    
    // POST /api/auth/login/azure
    [HttpPost("login/azure")]
    public async Task<IActionResult> LoginWithAzureAd([FromBody] AzureAdLoginRequestDTO request)
    {
        var result = await _authService.LoginWithAzureAdAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }
    
    // POST /api/auth/complete-tenant-setup
    [HttpPost("complete-tenant-setup")]
    [Authorize] // Requer token parcial
    public async Task<IActionResult> CompleteTenantSetup([FromBody] CompleteTenantSetupDTO request)
    {
        var tenantId = Guid.Parse(User.FindFirst("TenantId")?.Value);
        var result = await _authService.CompleteTenantSetupAsync(tenantId, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
```

#### **ProductsController.cs**

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize] // Todas as rotas exigem autenticação
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    
    // GET /api/products
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(new { Success = true, Data = products });
    }
    
    // GET /api/products/my-products
    [HttpGet("my-products")]
    public async Task<IActionResult> GetMyProducts()
    {
        var tenantId = Guid.Parse(User.FindFirst("TenantId")?.Value);
        var products = await _productService.GetMyProductsAsync(tenantId);
        return Ok(new { Success = true, Data = products });
    }
    
    // GET /api/products/has-access/{productId}
    [HttpGet("has-access/{productId}")]
    public async Task<IActionResult> HasAccess(Guid productId)
    {
        var tenantId = Guid.Parse(User.FindFirst("TenantId")?.Value);
        var hasAccess = await _productService.HasAccessToProductAsync(tenantId, productId);
        return Ok(new { Success = true, Data = hasAccess });
    }
}
```

---

## 🔐 Autenticação JWT

### **JWT Token Structure**

```json
{
  "sub": "user-guid",
  "email": "usuario@empresa.com",
  "name": "Nome do Usuário",
  "role": "Admin",
  "TenantId": "tenant-guid",
  "TenantName": "Empresa LTDA",
  "TenantStatus": "Ativo",
  "nbf": 1699999999,
  "exp": 1700086399,
  "iat": 1699999999,
  "iss": "LevverRH",
  "aud": "LevverRH"
}
```

### **Program.cs - JWT Configuration**

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "LevverRH",
            ValidAudience = "LevverRH",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]))
        };
    });
```

---

## 📊 Banco de Dados - Schema Isolation

### **Estratégia: Schema-based Multi-tenancy**

```sql
-- Schema Compartilhado (Tabelas Globais)
CREATE SCHEMA shared;

CREATE TABLE shared.tenants (...);
CREATE TABLE shared.users (...);
CREATE TABLE shared.products_catalog (...);
CREATE TABLE shared.tenant_products (...);

-- Schema por Tenant (Dados Isolados)
CREATE SCHEMA tenant_12345678-1234-1234-1234-123456789abc;

CREATE TABLE tenant_12345678-1234-1234-1234-123456789abc.candidatos (...);
```

---

## 🤖 Integração com IA (OpenAI)

### **Pacotes NuGet Instalados**

```xml
<PackageReference Include="Microsoft.Extensions.AI" Version="10.0.1" />
<PackageReference Include="Microsoft.Extensions.AI.OpenAI" Version="10.0.1-preview" />
<PackageReference Include="UglyToad.PdfPig" Version="0.1.9-alpha001-patch1" />
```

### **Configuração (appsettings.json)**

```json
{
  "OpenAI": {
    "ApiKey": "sk-proj-...",
    "Model": "gpt-4o-mini"
  }
}
```

### **Serviços de IA Implementados**

#### **1. JobAIService.cs** (Criação de Vagas Assistida)

```csharp
public class JobAIService : IJobAIService
{
    private readonly IChatClient _chatClient;
    
    public async Task<string> GetFirstQuestionAsync(string mensagemInicial)
    {
        var messages = new List<AIChatMessage>
        {
            new(ChatRole.System, SYSTEM_PROMPT),
            new(ChatRole.User, $"Usuário quer criar vaga: {mensagemInicial}")
        };
        
        var response = await _chatClient.GetResponseAsync(messages, new ChatOptions
        {
            ResponseFormat = ChatResponseFormat.Json
        });
        
        return ParseAIResponse(response.Text).Message;
    }
    
    public async Task<AIProcessingResult> ProcessUserResponseAsync(
        Job job, 
        List<ChatMessageItem> conversationHistory, 
        string userMessage)
    {
        var jobContext = BuildJobContext(job);
        
        var messages = new List<AIChatMessage>
        {
            new(ChatRole.System, SYSTEM_PROMPT),
            new(ChatRole.System, $"Estado atual:\n{jobContext}")
        };
        
        foreach (var msg in conversationHistory)
        {
            messages.Add(new AIChatMessage(
                msg.Role == "user" ? ChatRole.User : ChatRole.Assistant,
                msg.Content
            ));
        }
        
        messages.Add(new AIChatMessage(ChatRole.User, userMessage));
        
        var response = await _chatClient.GetResponseAsync(messages, options);
        var parsed = ParseAIResponse(response.Text);
        
        return new AIProcessingResult
        {
            AIResponse = parsed.Message,
            ExtractedFields = parsed.ExtractedFields,
            IsComplete = parsed.IsComplete,
            CompletionPercentage = parsed.CompletionPercentage
        };
    }
}
```

#### **2. CandidateAnalyzer.cs** (Análise de Currículos)

```csharp
public class CandidateAnalyzer : ICandidateAnalyzer
{
    private readonly IChatClient _chatClient;
    
    public async Task<CandidateAnalysisResult> AnalyzeAsync(
        string resumeText, 
        string jobRequirements)
    {
        var systemPrompt = @"
        Você é um especialista em análise de currículos.
        Retorne JSON com:
        {
          'scoreGeral': 0-100,
          'scoreTecnico': 0-100,
          'scoreExperiencia': 0-100,
          'justificativa': 'Análise detalhada...',
          'pontosFortes': 'Pontos positivos...',
          'pontosAtencao': 'Pontos de atenção...'
        }";
        
        var userPrompt = $@"
        REQUISITOS DA VAGA:
        {jobRequirements}
        
        CURRÍCULO:
        {resumeText}
        
        Analise e retorne JSON.";
        
        var messages = new List<AIChatMessage>
        {
            new(ChatRole.System, systemPrompt),
            new(ChatRole.User, userPrompt)
        };
        
        var response = await _chatClient.GetResponseAsync(messages, new ChatOptions
        {
            ResponseFormat = ChatResponseFormat.Json,
            Temperature = 0.1f,
            MaxOutputTokens = 4096
        });
        
        var tokensUsed = (int)(response.Usage?.TotalTokenCount ?? 0);
        var estimatedCost = tokensUsed / 1_000_000.0m * 5.0m; // $5/1M tokens
        
        return new CandidateAnalysisResult
        {
            Score = parsed.ScoreGeral,
            Summary = parsed.Justificativa,
            TokensUsed = tokensUsed,
            EstimatedCost = estimatedCost
        };
    }
}
```

#### **3. PdfExtractor.cs** (Extração de Texto de PDF)

```csharp
public class PdfExtractor : IPdfExtractor
{
    public async Task<string> ExtractTextAsync(byte[] pdfContent)
    {
        return await Task.Run(() =>
        {
            using var document = PdfDocument.Open(pdfContent);
            var textBuilder = new StringBuilder();
            
            foreach (var page in document.GetPages())
            {
                textBuilder.AppendLine($"--- Página {page.Number} ---");
                textBuilder.AppendLine(page.Text);
            }
            
            return textBuilder.ToString();
        });
    }
}
```

### **API Endpoints de IA**

```csharp
// POST /api/talents/applications/{id}/analyze
[HttpPost("{id}/analyze")]
[Authorize]
public async Task<IActionResult> AnalyzeCandidateWithAI(Guid id)
{
    var tenantId = Guid.Parse(User.FindFirst("TenantId")?.Value);
    var result = await _applicationService.AnalyzeCandidateWithAIAsync(id, tenantId);
    return Ok(new { Success = true, Data = result });
}
```

---

**Última Atualização**: 30 de Novembro de 2025
CREATE TABLE tenant_12345678-1234-1234-1234-123456789abc.vagas (...);
```

### **Vantagens**
- ✅ Isolamento total de dados por tenant
- ✅ Fácil backup/restore por tenant
- ✅ Escalabilidade (futuramente pode mover schema para outro DB)
- ✅ Segurança (queries não podem acessar dados de outros tenants)

---

**Última Atualização**: 16 de Novembro de 2025
