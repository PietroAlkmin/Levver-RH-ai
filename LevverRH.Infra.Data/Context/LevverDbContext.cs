using LevverRH.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LevverRH.Infra.Data.Context;

public class LevverDbContext : DbContext
{
    public LevverDbContext(DbContextOptions<LevverDbContext> options) : base(options)
    {
    }

    // DbSets - Tabelas
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<WhiteLabel> WhiteLabels { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<ProductCatalog> ProductCatalogs { get; set; }
    public DbSet<TenantProduct> TenantProducts { get; set; }
    public DbSet<TenantSubscription> TenantSubscriptions { get; set; }
    public DbSet<IntegrationCredentials> IntegrationCredentials { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar todas as configurações da pasta EntitiesConfiguration
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LevverDbContext).Assembly);
    }
}