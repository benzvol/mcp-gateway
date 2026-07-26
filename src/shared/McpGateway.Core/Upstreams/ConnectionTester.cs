using System.Diagnostics;

using McpGateway.Core.Domain;

using Microsoft.Extensions.Options;

namespace McpGateway.Core.Upstreams;

internal sealed class ConnectionTester : IConnectionTester
{
    private readonly IUpstreamConnector _connector;
    private readonly IOptions<ConnectionTestOptions> _options;
    private readonly SemaphoreSlim _concurrencyLimiter;

    public ConnectionTester(IUpstreamConnector connector, IOptions<ConnectionTestOptions> options)
    {
        _connector = connector;
        _options = options;
        _concurrencyLimiter = new SemaphoreSlim(options.Value.MaxConcurrentTests);
    }

    public async Task<ConnectionTestResult> TestAsync(Upstream upstream, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        await _concurrencyLimiter.WaitAsync(cancellationToken);
        try
        {
            using var timeoutCts = new CancellationTokenSource(_options.Value.Timeout);
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            try
            {
                await using var client = await _connector.ConnectAsync(upstream, linkedCts.Token);
                var tools = await client.ListToolsAsync(cancellationToken: linkedCts.Token);

                var discovered = tools
                    .Select(tool => new DiscoveredTool(tool.Name, tool.Description))
                    .ToArray();

                return ConnectionTestResult.Ok(discovered, stopwatch.Elapsed);
            }
            catch (Exception ex)
            {
                var kind = ConnectionFailureCategorizer.Categorize(ex, timeoutCts.Token);
                return ConnectionTestResult.Fail(kind, ex.Message, stopwatch.Elapsed);
            }
        }
        finally
        {
            _concurrencyLimiter.Release();
        }
    }
}
