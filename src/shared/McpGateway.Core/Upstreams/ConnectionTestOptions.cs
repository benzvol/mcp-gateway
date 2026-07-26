namespace McpGateway.Core.Upstreams;

public sealed class ConnectionTestOptions
{
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);

    public int MaxConcurrentTests { get; set; } = 8;
}
