using McpGateway.Core.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace McpGateway.Core.DependencyInjection;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddGatewayPersistence(string connectionString)
        {
            services.AddDbContext<GatewayDbContext>(options => options.ConfigureGateway(connectionString));
            return services;
        }

        public IServiceCollection AddGatewayDatabaseInitializer()
        {
            services.AddHostedService<DatabaseInitializer>();
            return services;
        }
    }
}