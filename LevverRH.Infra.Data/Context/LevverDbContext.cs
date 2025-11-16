using LevverRH.Domain.Entities;
using LevverRH.Domain.Entities.Talents;
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

    // DbSets - Talents Product
    public DbSet<Job> TalentsJobs { get; set; }
    public DbSet<Candidate> TalentsCandidates { get; set; }
    public DbSet<Application> TalentsApplications { get; set; }
    public DbSet<ChatMessage> TalentsChatMessages { get; set; }
    public DbSet<TalentsAuditLog> TalentsAuditLogs { get; set; }
    public DbSet<UsageMetric> TalentsUsageMetrics { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar todas as configurações da pasta EntitiesConfiguration
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LevverDbContext).Assembly);
    }
}