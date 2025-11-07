using LevverRH.Infra.Data.Context;
using LevverRH.Infra.IoC;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configura serialização JSON para usar camelCase (padrão JavaScript/TypeScript)
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        // Permite leitura de números como string
        options.JsonSerializerOptions.NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString;
    });

// Database
builder.Services.AddDbContext<LevverDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Infrastructure (Repositories, Services, AutoMapper, FluentValidation)
builder.Services.AddInfrastructure(builder.Configuration);

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
        ClockSkew = TimeSpan.Zero
    };
});

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    // Pol�tica para Admins
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    
    // Pol�tica para Admins e Recruiters
    options.AddPolicy("AdminOrRecruiter", policy => 
        policy.RequireRole("Admin", "Recruiter"));
    
    // Pol�tica para qualquer usu�rio autenticado
    options.AddPolicy("AuthenticatedUser", policy => 
        policy.RequireAuthenticatedUser());
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LevverRH API",
        Version = "v1",
        Description = "API do sistema LevverRH - Multi-tenant Recruitment System"
    });

    // JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT. Exemplo: Bearer {seu_token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// CORS para comunica��o com React
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactApp", policy =>
  {
    policy.WithOrigins(
         "http://localhost:5173",           // Vite dev server
     "https://localhost:5173"   // HTTPS se usar
      )
     .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();        // Importante para JWT
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "LevverRH API v1");
        c.DocumentTitle = "LevverRH API - Swagger";
    });
}

app.UseHttpsRedirection();

app.UseCors("ReactApp"); // Usar a pol�tica espec�fica do React

app.UseAuthentication();  // ? IMPORTANTE: Deve vir antes do UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();