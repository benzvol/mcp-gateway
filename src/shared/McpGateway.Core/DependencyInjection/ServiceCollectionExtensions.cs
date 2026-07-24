using McpGateway.Core.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace McpGateway.Core.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGatewayPersistence(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<GatewayDbContext>(options => options.ConfigureGateway(connectionString));
        return services;
    }

    public static IServiceCollection AddGatewayDatabaseInitializer(this IServiceCollection services)
    {
        services.AddHostedService<DatabaseInitializer>();
        return services;
    }
}