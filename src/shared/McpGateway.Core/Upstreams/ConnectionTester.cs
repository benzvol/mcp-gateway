using System.Diagnostics;

using McpGateway.Core.Domain;

using Microsoft.Extensions.Options;

namespace McpGateway.Core.Upstreams;

internal sealed class ConnectionTester(IUpstreamConnector connector, IOptions<ConnectionTestOptions> options)
    : IConnectionTester
{
    public async Task<ConnectionTestResult> TestAsync(Upstream upstream, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        using var timeoutCts = new CancellationTokenSource(options.Value.Timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        try
        {
            await using var client = await connector.ConnectAsync(upstream, linkedCts.Token);
            var tools = await client.ListToolsAsync(cancellationToken: linkedCts.Token);

            var discovered = tools
                .Select(tool => new DiscoveredTool(tool.Name, tool.Description))
                .ToArray();

            return ConnectionTestResult.Ok(discovered, stopwatch.Elapsed);
        }
        catch (Exception ex)
        {
            var kind = ConnectionFailureCategorizer.Categorize(ex);
            return ConnectionTestResult.Fail(kind, ex.Message, stopwatch.Elapsed);
        }
    }
}
