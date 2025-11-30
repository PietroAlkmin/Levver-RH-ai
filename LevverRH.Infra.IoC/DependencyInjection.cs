using FluentValidation;
using LevverRH.Application.Mappings;
using LevverRH.Application.Services.Implementations;
using LevverRH.Application.Services.Implementations.Talents;
using LevverRH.Application.Services.Interfaces;
using LevverRH.Application.Services.Interfaces.Talents;
using LevverRH.Application.Validators;
using LevverRH.Domain.Interfaces;
using LevverRH.Domain.Interfaces.Talents;
using LevverRH.Infra.Data.Context;
using LevverRH.Infra.Data.Repositories;
using LevverRH.Infra.Data.Repositories.Talents;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;

namespace LevverRH.Infra.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<LevverDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly("LevverRH.Infra.Data");
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null
                    );
                }
            ));

        // Repositories
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IWhiteLabelRepository, WhiteLabelRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IProductCatalogRepository, ProductCatalogRepository>();
        services.AddScoped<ITenantProductRepository, TenantProductRepository>();
        services.AddScoped<ITenantSubscriptionRepository, TenantSubscriptionRepository>();
        services.AddScoped<IIntegrationCredentialsRepository, IntegrationCredentialsRepository>();

        // Talents Repositories
        services.AddScoped<IJobRepository, JobRepository>();
        services.AddScoped<ICandidateRepository, CandidateRepository>();
        services.AddScoped<IApplicationRepository, ApplicationRepository>();
        services.AddScoped<IChatMessageRepository, ChatMessageRepository>();

        // AutoMapper
        services.AddAutoMapper(typeof(AuthMappingProfile));

        // FluentValidation
        services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

        // Application Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IStorageService, AzureBlobStorageService>();

        // Talents Services
        services.AddScoped<IJobService, JobService>();
        services.AddScoped<ICandidateService, CandidateService>();
        services.AddScoped<IApplicationService, ApplicationService>();
        services.AddScoped<IDashboardService, DashboardService>();

        // AI Services (OpenAI)
        services.AddSingleton<IChatClient>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var apiKey = config["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI:ApiKey não configurada no appsettings.json");
            var model = config["OpenAI:Model"] ?? "gpt-4o-mini";
            return new OpenAIClient(apiKey).GetChatClient(model).AsIChatClient();
        });
        services.AddScoped<IJobAIService, JobAIService>();

        return services;
    }
}