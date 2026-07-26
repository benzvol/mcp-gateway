namespace McpGateway.Core.Upstreams;

public sealed class ConnectionTestOptions
{
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(10);

    public int MaxConcurrentTests { get; init; } = 8;
}
