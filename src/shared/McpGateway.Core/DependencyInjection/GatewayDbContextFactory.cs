using McpGateway.Core.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace McpGateway.Core.DependencyInjection;

public class GatewayDbContextFactory : IDesignTimeDbContextFactory<GatewayDbContext>
{
    public GatewayDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<GatewayDbContext>();
        optionsBuilder.ConfigureGateway("Data Source=gateway.designtime.db");
        return new GatewayDbContext(optionsBuilder.Options);
    }
}