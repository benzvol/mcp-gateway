using McpGateway.Core.Domain;

using ModelContextProtocol.Client;

namespace McpGateway.Core.Upstreams;

public interface IUpstreamConnector
{
    Task<McpClient> ConnectAsync(Upstream upstream, CancellationToken cancellationToken = default);
}
