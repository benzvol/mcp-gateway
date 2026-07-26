using McpGateway.Core.Domain.Enums;

namespace McpGateway.Api.Upstreams;

public sealed record CreateUpstreamRequest(
    string Name,
    TransportKind Transport,
    string? Command,
    List<string>? Args,
    Dictionary<string, string>? Environment,
    string? Endpoint,
    Dictionary<string, string>? Headers,
    AuthKind AuthKind,
    string? AuthConfigJson,
    string? Secret,
    bool? Enabled
);

public sealed record UpdateUpstreamRequest(
    string Name,
    TransportKind Transport,
    string? Command,
    List<string>? Args,
    Dictionary<string, string>? Environment,
    string? Endpoint,
    Dictionary<string, string>? Headers,
    AuthKind AuthKind,
    string? AuthConfigJson,
    string? Secret,
    bool Enabled
);

public sealed record SetEnabledRequest(bool Enabled);

public sealed record UpstreamResponse(
    Guid Id,
    string Name,
    bool Enabled,
    TransportKind Transport,
    string? Command,
    IReadOnlyList<string> Args,
    IReadOnlyDictionary<string, string> Environment,
    string? Endpoint,
    IReadOnlyDictionary<string, string> Headers,
    AuthKind AuthKind,
    bool HasAuthConfig,
    bool HasSecret
);

public sealed record ToolPreview(string Name, string? Description);

public sealed record TestConnectionResponse(
    bool Success,
    string FailureKind,
    string? Message,
    double LatencyMs,
    IReadOnlyList<ToolPreview> Tools
);
