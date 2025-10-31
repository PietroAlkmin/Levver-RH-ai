using FluentValidation;
using LevverRH.Application.Mappings;
using LevverRH.Application.Services.Implementations;
using LevverRH.Application.Services.Interfaces;
using LevverRH.Application.Validators;
using LevverRH.Domain.Interfaces;
using LevverRH.Infra.Data.Context;
using LevverRH.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        services.AddScoped<ITenantSubscriptionRepository, TenantSubscriptionRepository>();
        services.AddScoped<IIntegrationCredentialsRepository, IntegrationCredentialsRepository>();

        // AutoMapper
        services.AddAutoMapper(typeof(AuthMappingProfile));

        // FluentValidation
        services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

        // Application Services
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}