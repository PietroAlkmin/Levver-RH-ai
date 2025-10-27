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
                b => b.MigrationsAssembly("LevverRH.Infra.Data")
            ));

        // Repositories
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IWhiteLabelRepository, WhiteLabelRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IProductCatalogRepository, ProductCatalogRepository>();
        services.AddScoped<ITenantSubscriptionRepository, TenantSubscriptionRepository>();
        services.AddScoped<IIntegrationCredentialsRepository, IntegrationCredentialsRepository>();

        return services;
    }
}