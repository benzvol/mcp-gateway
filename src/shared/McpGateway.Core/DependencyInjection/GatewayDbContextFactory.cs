using McpGateway.Core.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace McpGateway.Core.DependencyInjection;

public class GatewayDbContextFactory : IDesignTimeDbContextFactory<GatewayDbContext>
{
    public GatewayDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<GatewayDbContext>();
        optionsBuilder.UseSqlite("Data Source=gateway.designtime.db").UseSnakeCaseNamingConvention();
        return new GatewayDbContext(optionsBuilder.Options);
    }
}