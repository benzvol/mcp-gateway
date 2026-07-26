using McpGateway.Core.Persistence;
using McpGateway.Core.Upstreams;

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

        public IServiceCollection AddUpstreamConnectivity(Action<ConnectionTestOptions>? configure = null)
        {
            services.AddOptions<ConnectionTestOptions>().Configure(configure ?? (_ => { }));
            services.AddSingleton<IUpstreamConnector, UpstreamConnector>();
            services.AddSingleton<IConnectionTester, ConnectionTester>();
            return services;
        }
    }
}
