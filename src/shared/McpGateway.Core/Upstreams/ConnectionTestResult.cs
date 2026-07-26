namespace McpGateway.Core.Upstreams;

public enum ConnectionFailureKind
{
    None,
    Configuration,
    Unreachable,
    AuthFailed,
    Timeout,
    Protocol,
    Unknown,
}

public sealed record DiscoveredTool(string Name, string? Description);

public sealed record ConnectionTestResult
{
    public required bool Success { get; init; }

    public ConnectionFailureKind FailureKind { get; init; } = ConnectionFailureKind.None;

    public string? Message { get; init; }

    public required TimeSpan Latency { get; init; }

    public IReadOnlyList<DiscoveredTool> Tools { get; init; } = [];

    public static ConnectionTestResult Ok(IReadOnlyList<DiscoveredTool> tools, TimeSpan latency) => new()
    {
        Success = true,
        Latency = latency,
        Tools = tools,
    };

    public static ConnectionTestResult Fail(ConnectionFailureKind kind, string message, TimeSpan latency) => new()
    {
        Success = false,
        FailureKind = kind,
        Message = message,
        Latency = latency,
    };
}
