using McpGateway.Core.Domain;

namespace McpGateway.Core.Upstreams;

public interface IConnectionTester
{
    Task<ConnectionTestResult> TestAsync(Upstream upstream, CancellationToken cancellationToken = default);
}
