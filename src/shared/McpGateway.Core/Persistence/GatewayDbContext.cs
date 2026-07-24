using McpGateway.Core.Domain;

using Microsoft.EntityFrameworkCore;

namespace McpGateway.Core.Persistence;

public class GatewayDbContext(DbContextOptions<GatewayDbContext> options) : DbContext(options)
{
    public DbSet<Upstream> Upstreams => Set<Upstream>();

    public DbSet<ToolOverride> ToolOverrides => Set<ToolOverride>();

    public DbSet<GatewayClient> GatewayClients => Set<GatewayClient>();

    public DbSet<AuditEntry> AuditEntries => Set<AuditEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GatewayDbContext).Assembly);
    }
}